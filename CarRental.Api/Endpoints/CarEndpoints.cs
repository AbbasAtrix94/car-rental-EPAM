using CarRental.Api.Models;
using CarRental.Api.Providers;
using CarRental.Api.Services;

namespace CarRental.Api.Endpoints;

public static class CarEndpoints
{
    public static void MapCarEndpoints(this WebApplication app)
    {
        app.MapGet("/cars/search", async (
            string? pickup,
            string? from,
            string? to,
            string? category,
            IEnumerable<ICarRentalProvider> providers,
            BookingStore bookingStore) =>
        {
            if (string.IsNullOrWhiteSpace(pickup) || string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(to))
                return Results.BadRequest("pickup, from, and to are required query parameters.");

            if (!DateOnly.TryParse(from, out var fromDate) || !DateOnly.TryParse(to, out var toDate))
                return Results.BadRequest("Invalid date format. Use yyyy-MM-dd.");

            if (toDate <= fromDate)
                return Results.BadRequest("'to' date must be after 'from' date.");

            VehicleCategory? categoryFilter = null;
            if (!string.IsNullOrWhiteSpace(category) &&
                Enum.TryParse<VehicleCategory>(category, ignoreCase: true, out var parsed))
            {
                categoryFilter = parsed;
            }

            var request = new SearchRequest
            {
                PickupLocation = pickup,
                From = fromDate,
                To = toDate,
                Category = categoryFilter
            };

            var tasks = providers.Select(p => p.SearchAsync(request));
            var allResults = (await Task.WhenAll(tasks)).SelectMany(r => r).ToList();

            foreach (var result in allResults)
                bookingStore.StoreVehicleResult(result);

            return Results.Ok(allResults);
        });

        app.MapPost("/cars/book", (BookingRequest request, BookingStore bookingStore) =>
        {
            if (!bookingStore.TryGetVehicleResult(request.VehicleResultId, out var vehicle) || vehicle is null)
                return Results.UnprocessableEntity("Vehicle result not found. Please search again.");

            var (isValid, message) = DocumentValidationService.Validate(vehicle.PickupLocation, request.DocumentType);
            if (!isValid)
                return Results.UnprocessableEntity(message);

            var reference = $"SKY-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
            var confirmation = new BookingConfirmation
            {
                Reference = reference,
                Provider = vehicle.Provider,
                TotalPrice = vehicle.TotalPrice,
                CancellationPolicy = vehicle.CancellationPolicy,
                PickupLocation = vehicle.PickupLocation,
                From = vehicle.From,
                To = vehicle.To,
                Category = vehicle.Category
            };

            bookingStore.AddBooking(confirmation);
            return Results.Ok(confirmation);
        });

        app.MapGet("/cars/booking/{reference}", (string reference, BookingStore bookingStore) =>
        {
            if (!bookingStore.TryGetBooking(reference, out var confirmation) || confirmation is null)
                return Results.NotFound($"Booking '{reference}' not found.");

            return Results.Ok(confirmation);
        });
    }
}
