using BookingsApi.Repositories;
using BookingsApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add MVC controller support
builder.Services.AddControllers();

builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IBookingService, BookingService>();

var app = builder.Build();

// Map controller endpoints
app.MapControllers();

app.Run();
