using BookingsApi.Repositories;
using BookingsApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add MVC controller support
builder.Services.AddControllers();

builder.Services.AddSingleton<BookingRepository>();
builder.Services.AddSingleton<BookingService>();

var app = builder.Build();

// Map controller endpoints
app.MapControllers();

app.Run();
