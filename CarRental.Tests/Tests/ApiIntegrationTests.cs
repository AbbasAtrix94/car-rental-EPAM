using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using CarRental.Api.Models;
using Microsoft.AspNetCore.Mvc.Testing;

namespace CarRental.Tests.Tests;

public class ApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private static readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    public ApiIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Search_MissingPickup_Returns400()
    {
        var response = await _client.GetAsync("/cars/search?from=2024-06-10&to=2024-06-15");
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Search_ToBeforeFrom_Returns400()
    {
        var response = await _client.GetAsync(
            "/cars/search?pickup=London&from=2024-06-15&to=2024-06-10");
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Search_Valid_Returns200WithResults()
    {
        var response = await _client.GetAsync(
            "/cars/search?pickup=London&from=2024-06-10&to=2024-06-14");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var results = await response.Content.ReadFromJsonAsync<List<VehicleResult>>(_json);
        Assert.NotNull(results);
        Assert.NotEmpty(results);
    }

    [Fact]
    public async Task Book_Valid_Returns200WithReference()
    {
        var searchResponse = await _client.GetAsync(
            "/cars/search?pickup=London&from=2024-07-01&to=2024-07-05");
        var results = await searchResponse.Content.ReadFromJsonAsync<List<VehicleResult>>(_json);
        Assert.NotNull(results);
        Assert.NotEmpty(results);

        var vehicleId = results![0].Id;

        var bookRequest = new BookingRequest
        {
            VehicleResultId = vehicleId,
            DriverName = "Jane Smith",
            DocumentType = DocumentType.Passport,
            DocumentNumber = "P123456"
        };

        var bookResponse = await _client.PostAsJsonAsync("/cars/book", bookRequest, _json);

        Assert.Equal(HttpStatusCode.OK, bookResponse.StatusCode);
        var confirmation = await bookResponse.Content.ReadFromJsonAsync<BookingConfirmation>(_json);
        Assert.NotNull(confirmation);
        Assert.StartsWith("SKY-", confirmation!.Reference);
    }

    [Fact]
    public async Task Book_InvalidDocument_Returns422()
    {
        var searchResponse = await _client.GetAsync(
            "/cars/search?pickup=Paris&from=2024-07-01&to=2024-07-05");
        var results = await searchResponse.Content.ReadFromJsonAsync<List<VehicleResult>>(_json);
        Assert.NotNull(results);
        Assert.NotEmpty(results);

        var bookRequest = new BookingRequest
        {
            VehicleResultId = results![0].Id,
            DriverName = "John Doe",
            DocumentType = DocumentType.NationalId,
            DocumentNumber = "NID999"
        };

        var bookResponse = await _client.PostAsJsonAsync("/cars/book", bookRequest, _json);

        Assert.Equal(HttpStatusCode.UnprocessableEntity, bookResponse.StatusCode);
    }

    [Fact]
    public async Task GetBooking_Valid_Returns200()
    {
        var searchResponse = await _client.GetAsync(
            "/cars/search?pickup=Manchester&from=2024-08-01&to=2024-08-04");
        var results = await searchResponse.Content.ReadFromJsonAsync<List<VehicleResult>>(_json);

        var bookRequest = new BookingRequest
        {
            VehicleResultId = results![0].Id,
            DriverName = "Alice",
            DocumentType = DocumentType.NationalId,
            DocumentNumber = "NID111"
        };
        var bookResponse = await _client.PostAsJsonAsync("/cars/book", bookRequest, _json);
        var confirmation = await bookResponse.Content.ReadFromJsonAsync<BookingConfirmation>(_json);

        var getResponse = await _client.GetAsync($"/cars/booking/{confirmation!.Reference}");

        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var retrieved = await getResponse.Content.ReadFromJsonAsync<BookingConfirmation>(_json);
        Assert.Equal(confirmation.Reference, retrieved!.Reference);
    }

    [Fact]
    public async Task GetBooking_Nonexistent_Returns404()
    {
        var response = await _client.GetAsync("/cars/booking/SKY-DOESNOTEXIST");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
