# AI Prompts & Decisions Log

## Tool Used
GitHub Copilot (Claude Sonnet 4.6) via GitHub Copilot CLI

## Session Overview
- Model: claude-sonnet-4.6
- Date: 2026-06-20 to 2026-06-21
- Approximate input tokens: ~14,000
- Approximate output tokens: ~22,000

## Key Prompts

### 1. Initial Analysis & Planning
**Prompt:** "Read the PDF challenge brief and build the complete car rental app..."
**Decision:** Used spec.md-first approach as required by the brief. spec.md was created before any .cs or .jsx files.

### 2. Backend Architecture
**Decision:** Chose Minimal API over Controller-based for simplicity and .NET 9 idioms. Used DI with `IEnumerable<ICarRentalProvider>` to support open/closed principle — adding a 3rd provider requires zero core changes. Both providers registered via `AddTransient<ICarRentalProvider, XProvider>()`.

### 3. Pricing Logic
**Decision:** BudgetWheels pricing iterates nightly, not multiplicative, to handle mixed weekday/weekend rentals correctly. `DayOfWeek.Friday=5`, `Saturday=6`, `Sunday=0`. The `PricingService.CalculateBudgetWheelsTotal` method is `static` so it can be called from the provider without DI overhead and tested directly without mocking.

### 4. Vehicle Result Cache Design
**Decision:** VehicleResult carries `PickupLocation`, `From`, and `To` from the search context. When results are returned from `/cars/search`, they're cached in `BookingStore._vehicleResults` (keyed by GUID ID). The `/cars/book` endpoint looks up the vehicle to reconstruct a `BookingConfirmation` — no round-trip to the frontend needed.

### 5. Document Validation
**Decision:** Unknown cities default to international (require Passport) — safer assumption for a travel platform. `DocumentValidationService` uses `HashSet<string>` with `OrdinalIgnoreCase` comparer for O(1) lookup.

### 6. Frontend State Management
**Decision:** Simple `useState` in `App.jsx` rather than Redux/Zustand — the state is shallow and flows one-way through four views: search → results → booking → confirmation. Added client-side doc-type pre-filtering (hide NationalId option for international locations) for better UX before the server validates.

### 7. NuGet Source Conflict
**Decision:** Created a root-level `NuGet.config` to clear all package sources and only use `nuget.org`. This resolved 401 errors from a private Azure Artifacts feed present in the global NuGet config.

### 8. Vite Template Issue
**Decision:** Vite v9's default `--template react` scaffolds a vanilla TypeScript project (no React). Fixed by manually installing `react`, `react-dom`, and `@vitejs/plugin-react`, creating `vite.config.js` with the React plugin, and removing `tsc &&` from the build script since JSX (not TSX) is used.

### 9. Test Strategy
**Decision:** Unit tests for pure logic (pricing, validation, provider behaviour), integration tests via `WebApplicationFactory<Program>` for full API contract verification. `public partial class Program {}` added to `Program.cs` to enable `WebApplicationFactory` to reference the entry point.

### 10. Route-Based Navigation (React Router)
**Prompt:** "After search vehicles, the form should load in a new component with different route."
**Decision:** Replaced the `view` state-machine in `App.jsx` with `react-router-dom` v7 `BrowserRouter` + `Routes`. Created a `pages/` layer that sits between `App.jsx` and the existing presentational `components/`. Each page owns its route, its async logic, and navigation:
- `/` → `SearchPage` — calls `searchCars()`, navigates to `/results` with `{ state: { results, searchParams } }`
- `/results` → `ResultsPage` — reads `location.state`, navigates to `/booking` with `{ state: { vehicle } }`
- `/booking` → `BookingPage` — calls `bookCar()`, navigates to `/confirmation` with `{ state: { confirmation } }`
- `/confirmation` → `ConfirmationPage` — reads `location.state`, "Search Again" navigates to `/`

State travels forward via `location.state` on each `navigate()` call. Every page guards against direct URL access by checking `if (!state?.vehicle)` and redirecting with `<Navigate to="/" replace />`. The existing presentational components were left entirely unchanged — only the container layer changed.

**Rationale for `location.state` over URL params:** Vehicle results and confirmations are complex objects (multiple fields including arrays). Encoding them in query strings would be unwieldy and expose internal data in the URL. `location.state` keeps the URL clean (`/results`, `/booking`) while carrying full objects. The tradeoff is that state is lost on hard refresh — the guard handles this gracefully by redirecting to search.

### 11. Document Type Dropdown Bug Fix
**Prompt:** "Document type dropdown is only loading Passport."
**Root cause:** `BookingForm.jsx` called `isInternational(vehicle.pickupLocation)`. When `pickupLocation` was `undefined` (e.g. due to a camelCase/PascalCase mismatch or any data gap), the function defaulted to `true` (international), hiding the National ID option entirely via conditional rendering.
**Decision:** Three-part fix:
1. Added `getPickupLocation(vehicle)` helper that checks both `vehicle.pickupLocation` and `vehicle.PickupLocation` — defensive against JSON serialization casing differences.
2. Changed `isInternational('')` to return `false` (unknown city → don't restrict) rather than `true`.
3. Changed UX from _hiding_ National ID to _disabling_ it with a descriptive label `"National ID (not accepted for international pickup)"`. Both options are always visible; the submit guard remains as the enforcement layer. This prevents the option from silently disappearing when the location detection has any ambiguity.
