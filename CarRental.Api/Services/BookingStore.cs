using System.Collections.Concurrent;
using CarRental.Api.Models;

namespace CarRental.Api.Services;

public class BookingStore
{
    private readonly ConcurrentDictionary<string, BookingConfirmation> _bookings = new();
    private readonly ConcurrentDictionary<string, VehicleResult> _vehicleResults = new();

    public void StoreVehicleResult(VehicleResult result) =>
        _vehicleResults[result.Id] = result;

    public bool TryGetVehicleResult(string id, out VehicleResult? result) =>
        _vehicleResults.TryGetValue(id, out result);

    public void AddBooking(BookingConfirmation confirmation) =>
        _bookings[confirmation.Reference] = confirmation;

    public bool TryGetBooking(string reference, out BookingConfirmation? confirmation) =>
        _bookings.TryGetValue(reference, out confirmation);
}
