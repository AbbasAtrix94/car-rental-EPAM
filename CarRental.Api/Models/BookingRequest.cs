namespace CarRental.Api.Models;

public class BookingRequest
{
    public string VehicleResultId { get; set; } = string.Empty;
    public string DriverName { get; set; } = string.Empty;
    public DocumentType DocumentType { get; set; }
    public string DocumentNumber { get; set; } = string.Empty;
}
