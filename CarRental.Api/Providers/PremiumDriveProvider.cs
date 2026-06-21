using CarRental.Api.Models;
using CarRental.Api.Services;

namespace CarRental.Api.Providers;

public class PremiumDriveProvider : ICarRentalProvider
{
    private static readonly Dictionary<VehicleCategory, decimal> Rates = new()
    {
        { VehicleCategory.Economy, 45m },
        { VehicleCategory.Compact, 55m },
        { VehicleCategory.SUV,     85m },
        { VehicleCategory.Minivan, 75m }
    };

    public Task<IEnumerable<VehicleResult>> SearchAsync(SearchRequest request)
    {
        var days = request.To.DayNumber - request.From.DayNumber;
        var categories = request.Category.HasValue
            ? new[] { request.Category.Value }
            : Enum.GetValues<VehicleCategory>();

        var results = categories.Select(cat => new VehicleResult
        {
            Id = Guid.NewGuid().ToString(),
            Provider = "PremiumDrive",
            Category = cat,
            PerDayRate = Rates[cat],
            TotalPrice = Math.Round(Rates[cat] * days, 2),
            CancellationPolicy = "Free cancellation up to 48 hours before pickup",
            IncludesInsurance = true,
            PickupLocation = request.PickupLocation,
            From = request.From,
            To = request.To
        });

        return Task.FromResult<IEnumerable<VehicleResult>>(results.ToList());
    }
}
