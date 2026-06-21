namespace CarRental.Api.Services;

public static class PricingService
{
    /// <summary>
    /// Calculates BudgetWheels total by iterating each night individually.
    /// Friday, Saturday, and Sunday nights carry a 1.2× surcharge.
    /// </summary>
    public static decimal CalculateBudgetWheelsTotal(decimal baseRate, DateOnly from, DateOnly to)
    {
        decimal total = 0m;
        var current = from;

        while (current < to)
        {
            var dow = current.DayOfWeek;
            bool isWeekendNight = dow == DayOfWeek.Friday
                               || dow == DayOfWeek.Saturday
                               || dow == DayOfWeek.Sunday;

            total += isWeekendNight ? baseRate * 1.2m : baseRate;
            current = current.AddDays(1);
        }

        return Math.Round(total, 2);
    }
}
