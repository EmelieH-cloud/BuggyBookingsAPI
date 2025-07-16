using BookingsApi.Repositories;
using BookingsApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add MVC controller support
builder.Services.AddControllers();

builder.Services.AddScoped<BookingService>();
builder.Services.AddScoped<BookingRepository>();

var app = builder.Build();

// Map controller endpoints
app.MapControllers();

app.Run();
