using CarRental.Api.Services;

namespace CarRental.Tests.Tests;

public class PricingServiceTests
{
    [Fact]
    public void WeekdayOnly_NoSurcharge()
    {
        // Mon 22 Jan 2024 → Thu 25 Jan 2024 = 3 nights (Mon, Tue, Wed)
        var from = new DateOnly(2024, 1, 22); // Monday
        var to   = new DateOnly(2024, 1, 25); // Thursday

        decimal total = PricingService.CalculateBudgetWheelsTotal(10m, from, to);

        Assert.Equal(30m, total); // 3 × 10 = 30
    }

    [Fact]
    public void SpanningFridayNight_AppliesSurchargeOnFridayOnly()
    {
        // Mon 22 Jan → Sat 27 Jan = 5 nights (Mon, Tue, Wed, Thu, Fri)
        var from = new DateOnly(2024, 1, 22); // Monday
        var to   = new DateOnly(2024, 1, 27); // Saturday

        decimal total = PricingService.CalculateBudgetWheelsTotal(10m, from, to);

        // 4 weekday nights × 10 + 1 Friday night × 12 = 52
        Assert.Equal(52m, total);
    }

    [Fact]
    public void FullWeekendFriToMon_AllThreeNightsSurcharged()
    {
        // Fri 26 Jan 2024 → Mon 29 Jan 2024 = 3 nights (Fri, Sat, Sun)
        var from = new DateOnly(2024, 1, 26); // Friday
        var to   = new DateOnly(2024, 1, 29); // Monday

        decimal total = PricingService.CalculateBudgetWheelsTotal(10m, from, to);

        // 3 weekend nights × 12 = 36
        Assert.Equal(36m, total);
    }

    [Fact]
    public void TotalIsSumOfNights_NotMultiplicative()
    {
        // Fri 26 Jan → Sun 28 Jan = 2 nights (Fri, Sat)
        var from = new DateOnly(2024, 1, 26); // Friday
        var to   = new DateOnly(2024, 1, 28); // Sunday

        decimal total = PricingService.CalculateBudgetWheelsTotal(10m, from, to);

        // Sum: Fri(12) + Sat(12) = 24  (not 10 × 1.2 × 1.2 = 14.4)
        Assert.Equal(24m, total);
    }
}
