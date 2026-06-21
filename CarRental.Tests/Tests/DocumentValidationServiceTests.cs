using CarRental.Api.Models;
using CarRental.Api.Services;

namespace CarRental.Tests.Tests;

public class DocumentValidationServiceTests
{
    [Fact]
    public void InternationalCity_Passport_IsValid()
    {
        var (isValid, message) = DocumentValidationService.Validate("Paris", DocumentType.Passport);

        Assert.True(isValid);
    }

    [Fact]
    public void InternationalCity_NationalId_IsInvalid()
    {
        var (isValid, message) = DocumentValidationService.Validate("Tokyo", DocumentType.NationalId);

        Assert.False(isValid);
        Assert.Contains("Passport", message);
    }

    [Fact]
    public void DomesticCity_NationalId_IsValid()
    {
        var (isValid, _) = DocumentValidationService.Validate("London", DocumentType.NationalId);

        Assert.True(isValid);
    }

    [Fact]
    public void DomesticCity_Passport_IsValid()
    {
        var (isValid, _) = DocumentValidationService.Validate("Manchester", DocumentType.Passport);

        Assert.True(isValid);
    }

    [Fact]
    public void UnknownCity_TreatedAsInternational_RequiresPassport()
    {
        // Unknown city → treated as international → NationalId rejected
        var (isValid, message) = DocumentValidationService.Validate("Berlin", DocumentType.NationalId);

        Assert.False(isValid);
        Assert.Contains("Passport", message);
    }

    [Fact]
    public void UnknownCity_WithPassport_IsValid()
    {
        var (isValid, _) = DocumentValidationService.Validate("Berlin", DocumentType.Passport);

        Assert.True(isValid);
    }
}
