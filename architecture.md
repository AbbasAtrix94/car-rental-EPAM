# Architecture — Car Rental Availability (SkyRoute)

## Overview

The system is split into two independently runnable projects:

| Project | Tech | Port |
|---|---|---|
| `CarRental.Api` | .NET 8 Minimal API (C#) | 5186 |
| `car-rental-ui` | React 18 + Vite | 5173 |
| `CarRental.Tests` | xUnit (.NET 8) | — |

Communication is over HTTP/JSON. The UI calls the API directly via `axios`; no BFF or gateway layer exists.

```
┌─────────────────────┐        HTTP/JSON        ┌──────────────────────┐
│   car-rental-ui     │  ──────────────────────▶ │   CarRental.Api      │
│   React 18 (Vite)   │  ◀──────────────────────  │   .NET 8 Minimal API │
│   :5173             │                           │   :5186              │
└─────────────────────┘                           └──────────────────────┘
```

---

## API Architecture (`CarRental.Api`)

### Architectural Style
**Minimal API** — no MVC controllers. All route handlers are registered via a single extension method. The design follows a **vertical slice** layout grouped by concern (Models, Providers, Services, Endpoints).

### Folder Structure

```
CarRental.Api/
├── Program.cs                        ← Composition root & DI wiring
├── Endpoints/
│   └── CarEndpoints.cs               ← All 3 route handlers
├── Models/
│   ├── VehicleCategory.cs            ← Enum: Economy, Compact, SUV, Minivan
│   ├── DocumentType.cs               ← Enum: Passport, NationalId
│   ├── SearchRequest.cs              ← PickupLocation, From, To, Category?
│   ├── VehicleResult.cs              ← Unified vehicle DTO with Id, pricing, policy
│   ├── BookingRequest.cs             ← VehicleResultId, DriverName, DocumentType, DocumentNumber
│   └── BookingConfirmation.cs        ← Reference, Provider, TotalPrice, policy, dates
├── Providers/
│   ├── ICarRentalProvider.cs         ← Abstraction: SearchAsync(SearchRequest)
│   ├── PremiumDriveProvider.cs       ← Flat daily rate, always available, comprehensive insurance
│   └── BudgetWheelsProvider.cs       ← Weekend surcharge, SUV unavailable, basic insurance
└── Services/
    ├── PricingService.cs             ← Static: CalculateBudgetWheelsTotal(rate, from, to)
    ├── DocumentValidationService.cs  ← Static: Validate(location, docType)
    └── BookingStore.cs               ← Singleton: ConcurrentDictionary for results & bookings
```

### Dependency Injection

Registered in `Program.cs`:

```
Singleton  →  BookingStore
Transient  →  ICarRentalProvider  (PremiumDriveProvider)
Transient  →  ICarRentalProvider  (BudgetWheelsProvider)
```

Both providers are registered under the **same interface**. The endpoint resolves `IEnumerable<ICarRentalProvider>` and fans out to all registered providers in parallel — adding a third provider requires only a single new `AddTransient` line. No core logic changes.

### Layer Diagram

```
┌──────────────────────────────────────────────────┐
│                  Endpoints Layer                  │
│              CarEndpoints.cs                      │
│  GET /cars/search  POST /cars/book  GET /booking  │
└────────────────┬─────────────┬────────────────────┘
                 │             │
    ┌────────────▼──┐   ┌──────▼─────────────────┐
    │  Providers    │   │      Services           │
    │               │   │                         │
    │ ICarRental    │   │  PricingService         │
    │ Provider      │   │  (static, pure)         │
    │               │   │                         │
    │ PremiumDrive  │   │  DocumentValidation     │
    │ Provider      │   │  Service (static, pure) │
    │               │   │                         │
    │ BudgetWheels  │   │  BookingStore           │
    │ Provider      │   │  (singleton, in-memory) │
    └───────┬───────┘   └─────────────────────────┘
            │
    ┌───────▼───────┐
    │    Models     │
    │  (pure DTOs)  │
    └───────────────┘
```

### Key Design Decisions

| Decision | Rationale |
|---|---|
| `IEnumerable<ICarRentalProvider>` DI | Open/closed principle — new providers need zero core changes |
| `Task.WhenAll(...)` fan-out | Both providers queried in parallel for minimum latency |
| `PricingService` as static | Pure function with no state; no benefit to instantiation |
| `BookingStore` as singleton | In-memory state must survive the request lifetime |
| `ConcurrentDictionary` in `BookingStore` | Thread-safe without explicit locking under concurrent requests |
| `VehicleResult.Id` (GUID) | Links the search result to the booking without a database |

### API Endpoints

| Method | Route | Description | Error codes |
|---|---|---|---|
| `GET` | `/cars/search` | Query both providers, merge, return unified list | 400 (missing params, bad date order) |
| `POST` | `/cars/book` | Validate document, store booking, return reference | 422 (doc mismatch, result not found) |
| `GET` | `/cars/booking/{reference}` | Retrieve booking by reference number | 404 (not found) |

### Provider Behaviour

| | PremiumDrive | BudgetWheels |
|---|---|---|
| Pricing | `rate × days` (flat) | Per-night loop with weekend surcharge |
| Weekend surcharge | None | Fri / Sat / Sun nights × 1.2 |
| Availability | Always all 4 categories | SUV marked unavailable (filtered) |
| Insurance | Comprehensive (included) | Basic only |
| Cancellation | Free up to 48 h | Non-refundable |

### Document Validation Rules

```
Pickup location
    │
    ├── Domestic  (London, Manchester)
    │       └── Passport  ✓
    │       └── NationalId ✓
    │
    └── International / Unknown  (Paris, New York, Tokyo, anything else)
            └── Passport  ✓
            └── NationalId  ✗  →  422 "International pickups require a Passport."
```

---

## Frontend Architecture (`car-rental-ui`)

### Architectural Style
**Single-Page Application** with **React Router v7** (`react-router-dom`). Each screen is a dedicated route with its own page component. State is passed between routes via React Router's `location.state` (navigate-with-state). Presentational components remain in `components/` and are decoupled from routing — page wrappers in `pages/` handle navigation logic.

### Folder Structure

```
car-rental-ui/
├── index.html
└── src/
    ├── main.jsx                   ← React root mount
    ├── App.jsx                    ← BrowserRouter + Routes (thin router shell)
    ├── index.css                  ← Global styles
    ├── api/
    │   └── carRentalApi.js        ← Axios client, 3 exported functions
    ├── pages/                     ← Route-level components (own async logic & navigation)
    │   ├── SearchPage.jsx         ← Route: /          — calls API, navigates to /results
    │   ├── ResultsPage.jsx        ← Route: /results   — reads location.state, navigates to /booking
    │   ├── BookingPage.jsx        ← Route: /booking   — calls API, navigates to /confirmation
    │   └── ConfirmationPage.jsx   ← Route: /confirmation — reads location.state
    └── components/                ← Pure presentational components (no routing knowledge)
        ├── SearchForm.jsx         ← Pickup, dates, category; client-side date validation
        ├── ResultsList.jsx        ← Grid of VehicleCards; sort-by-price toggle; empty/error state
        ├── VehicleCard.jsx        ← Provider badge, rates, policy, insurance indicator, Book button
        ├── BookingForm.jsx        ← Driver name, document type/number; client-side doc validation
        └── ConfirmationPage.jsx   ← Reference number, summary, "Search Again" reset
```

### Route Map

```
BrowserRouter
    │
    ├── /               →  SearchPage
    ├── /results        →  ResultsPage
    ├── /booking        →  BookingPage
    ├── /confirmation   →  ConfirmationPage
    └── *               →  <Navigate to="/" replace />
```

### Navigation Flow

```
         navigate('/results', { state: { results, searchParams } })
  ┌──────────────────────────────────────────▶ /results
  │                                               │
 /                                  navigate('/booking', { state: { vehicle } })
  │                                               │
  │                                            /booking
  │                                               │
  │                              navigate('/confirmation', { state: { confirmation } })
  │                                               │
  └───────────────────── /confirmation ◀──────────┘
        navigate('/')
```

State is never held in a shared store — it travels forward via `location.state` on each navigation. Each page has a **guard**: if it receives no state (e.g. direct URL access or page refresh), it redirects to `/` with `<Navigate to="/" replace />`.

### Page Component Responsibilities

| Page | Route | Owns | API call |
|---|---|---|---|
| `SearchPage` | `/` | `loading`, `error` state | `searchCars()` → navigate to `/results` |
| `ResultsPage` | `/results` | Reads `location.state.results` | None |
| `BookingPage` | `/booking` | `loading`, `error` state; reads `location.state.vehicle` | `bookCar()` → navigate to `/confirmation` |
| `ConfirmationPage` | `/confirmation` | Reads `location.state.confirmation` | None |

### Presentational Component Responsibilities

| Component | Inputs (props) | Outputs (callbacks) |
|---|---|---|
| `SearchForm` | `loading`, `error` | `onSearch(params)` |
| `ResultsList` | `results`, `searchParams` | `onBook(vehicle)`, `onBack()` |
| `VehicleCard` | `vehicle` | `onBook(vehicle)` |
| `BookingForm` | `vehicle`, `loading`, `error` | `onConfirm(data)`, `onBack()` |
| `ConfirmationPage` | `confirmation` | `onSearchAgain()` |

All API calls are made in page components only — presentational components have no routing or HTTP knowledge.

### API Client (`carRentalApi.js`)

```
axios instance (baseURL: http://localhost:5186)
    │
    ├── searchCars({ pickup, from, to, category })
    │       └── GET /cars/search?pickup=&from=&to=&category=
    │
    ├── bookCar({ vehicleResultId, driverName, documentType, documentNumber })
    │       └── POST /cars/book
    │
    └── getBooking(reference)
            └── GET /cars/booking/{reference}
```

### Client-Side Validation

| Rule | Where enforced |
|---|---|
| `to` date must be after `from` | `SearchForm` — blocks submit |
| International location → National ID option disabled in dropdown | `BookingForm` — `disabled` attribute + informational hint; submit guard as fallback |
| All booking fields required | `BookingForm` — submit guard |

> **Note on document type dropdown:** Both `Passport` and `National ID` options are always visible. For international pickups, `National ID` is rendered with `disabled` and a label suffix `"(not accepted for international pickup)"`. This prevents the option from disappearing, which previously caused user confusion when `pickupLocation` was undefined. The submit handler enforces the rule as a second guard.

---

## Test Architecture (`CarRental.Tests`)

### Test Pyramid

```
       ┌─────────────────┐
       │  Integration     │  7 tests  — WebApplicationFactory, full HTTP stack
       ├─────────────────┤
       │  Provider        │  4 tests  — stub behaviour & filtering
       ├─────────────────┤
       │  Document Valid. │  6 tests  — location/document combinations
       ├─────────────────┤
       │  Pricing         │  4 tests  — surcharge arithmetic
       └─────────────────┘
                21 tests total
```

### Test Files

| File | Scope | Technique |
|---|---|---|
| `PricingServiceTests.cs` | `PricingService.CalculateBudgetWheelsTotal` | Pure unit, no mocks |
| `DocumentValidationServiceTests.cs` | `DocumentValidationService.Validate` | Pure unit, no mocks |
| `ProviderTests.cs` | `PremiumDriveProvider`, `BudgetWheelsProvider` | Direct instantiation |
| `ApiIntegrationTests.cs` | All 3 HTTP endpoints | `WebApplicationFactory<Program>` |

Integration tests use the real DI container and in-memory `BookingStore` — no mocking, no test doubles.

---

## Cross-Cutting Concerns

| Concern | Approach |
|---|---|
| CORS | `WithOrigins("http://localhost:5173")` in `Program.cs` |
| Error responses | `Results.BadRequest`, `Results.UnprocessableEntity`, `Results.NotFound` |
| Thread safety | `ConcurrentDictionary` in `BookingStore` |
| Extensibility | `ICarRentalProvider` + DI — new providers via one `AddTransient` call |
| No persistence | `BookingStore` is in-memory only; data lost on restart (by design) |
| No auth | Out of scope per challenge brief |
