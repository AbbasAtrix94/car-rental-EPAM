namespace CarRental.Api.Models;

public class VehicleResult
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Provider { get; set; } = string.Empty;
    public VehicleCategory Category { get; set; }
    public decimal PerDayRate { get; set; }
    public decimal TotalPrice { get; set; }
    public string CancellationPolicy { get; set; } = string.Empty;
    public bool IncludesInsurance { get; set; }

    // Search context carried for booking
    public string PickupLocation { get; set; } = string.Empty;
    public DateOnly From { get; set; }
    public DateOnly To { get; set; }
}
