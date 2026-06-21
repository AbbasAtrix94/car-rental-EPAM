using System.Text.Json.Serialization;
using CarRental.Api.Endpoints;
using CarRental.Api.Providers;
using CarRental.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddSingleton<BookingStore>();
builder.Services.AddTransient<ICarRentalProvider, PremiumDriveProvider>();
builder.Services.AddTransient<ICarRentalProvider, BudgetWheelsProvider>();

var app = builder.Build();

app.UseCors();
app.MapCarEndpoints();
app.Run();

public partial class Program { }
