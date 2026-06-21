using CarRental.Api.Models;
using CarRental.Api.Providers;

namespace CarRental.Tests.Tests;

public class ProviderTests
{
    private static SearchRequest MakeRequest(VehicleCategory? category = null) => new()
    {
        PickupLocation = "London",
        From = new DateOnly(2024, 6, 3),  // Monday
        To   = new DateOnly(2024, 6, 7),  // Friday
        Category = category
    };

    [Fact]
    public async Task PremiumDrive_AlwaysReturnsResults()
    {
        var provider = new PremiumDriveProvider();
        var results = (await provider.SearchAsync(MakeRequest())).ToList();

        Assert.NotEmpty(results);
        Assert.Equal(4, results.Count); // all 4 categories
        Assert.All(results, r => Assert.Equal("PremiumDrive", r.Provider));
    }

    [Fact]
    public async Task BudgetWheels_FiltersUnavailableVehicles()
    {
        var provider = new BudgetWheelsProvider();
        var results = (await provider.SearchAsync(MakeRequest())).ToList();

        // SUV is unavailable for BudgetWheels
        Assert.DoesNotContain(results, r => r.Category == VehicleCategory.SUV);
    }

    [Fact]
    public async Task BudgetWheels_CategoryFilter_Works()
    {
        var provider = new BudgetWheelsProvider();

        // Economy is available
        var economyResults = (await provider.SearchAsync(MakeRequest(VehicleCategory.Economy))).ToList();
        Assert.Single(economyResults);
        Assert.Equal(VehicleCategory.Economy, economyResults[0].Category);

        // SUV is unavailable — filter returns empty
        var suvResults = (await provider.SearchAsync(MakeRequest(VehicleCategory.SUV))).ToList();
        Assert.Empty(suvResults);
    }

    [Fact]
    public async Task PremiumDrive_CategoryFilter_ReturnsOnlyRequestedCategory()
    {
        var provider = new PremiumDriveProvider();
        var results = (await provider.SearchAsync(MakeRequest(VehicleCategory.SUV))).ToList();

        Assert.Single(results);
        Assert.Equal(VehicleCategory.SUV, results[0].Category);
    }
}
