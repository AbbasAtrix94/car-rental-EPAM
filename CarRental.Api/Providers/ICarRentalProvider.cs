using CarRental.Api.Models;

namespace CarRental.Api.Providers;

public interface ICarRentalProvider
{
    Task<IEnumerable<VehicleResult>> SearchAsync(SearchRequest request);
}
