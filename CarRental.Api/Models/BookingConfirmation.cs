namespace CarRental.Api.Models;

public class BookingConfirmation
{
    public string Reference { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty;
    public decimal TotalPrice { get; set; }
    public string CancellationPolicy { get; set; } = string.Empty;
    public string PickupLocation { get; set; } = string.Empty;
    public DateOnly From { get; set; }
    public DateOnly To { get; set; }
    public VehicleCategory Category { get; set; }
}
