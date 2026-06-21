using CarRental.Api.Models;

namespace CarRental.Api.Services;

public static class DocumentValidationService
{
    private static readonly HashSet<string> DomesticCities =
        new(StringComparer.OrdinalIgnoreCase) { "London", "Manchester" };

    private static readonly HashSet<string> InternationalCities =
        new(StringComparer.OrdinalIgnoreCase) { "Paris", "New York", "Tokyo" };

    public static (bool isValid, string message) Validate(string pickupLocation, DocumentType documentType)
    {
        bool isDomestic = DomesticCities.Contains(pickupLocation);

        if (!isDomestic && documentType != DocumentType.Passport)
            return (false, "International pickups require a Passport. National ID is not accepted.");

        return (true, "Valid");
    }

    public static bool IsDomestic(string pickupLocation) => DomesticCities.Contains(pickupLocation);
}
