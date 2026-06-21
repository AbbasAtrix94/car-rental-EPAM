using CarRental.Api.Models;
using CarRental.Api.Services;

namespace CarRental.Api.Providers;

public class BudgetWheelsProvider : ICarRentalProvider
{
    private static readonly Dictionary<VehicleCategory, decimal> Rates = new()
    {
        { VehicleCategory.Economy, 32m },
        { VehicleCategory.Compact, 42m },
        { VehicleCategory.SUV,     68m },
        { VehicleCategory.Minivan, 58m }
    };

    // SUV is marked unavailable to demonstrate filtering behaviour
    private static readonly HashSet<VehicleCategory> UnavailableCategories =
        new() { VehicleCategory.SUV };

    public Task<IEnumerable<VehicleResult>> SearchAsync(SearchRequest request)
    {
        var categories = request.Category.HasValue
            ? new[] { request.Category.Value }
            : Enum.GetValues<VehicleCategory>();

        var results = categories
            .Where(cat => !UnavailableCategories.Contains(cat))
            .Select(cat => new VehicleResult
            {
                Id = Guid.NewGuid().ToString(),
                Provider = "BudgetWheels",
                Category = cat,
                PerDayRate = Rates[cat],
                TotalPrice = PricingService.CalculateBudgetWheelsTotal(Rates[cat], request.From, request.To),
                CancellationPolicy = "Non-refundable",
                IncludesInsurance = false,
                PickupLocation = request.PickupLocation,
                From = request.From,
                To = request.To
            });

        return Task.FromResult<IEnumerable<VehicleResult>>(results.ToList());
    }
}
