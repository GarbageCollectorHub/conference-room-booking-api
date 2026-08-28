using Microsoft.EntityFrameworkCore;
using RoomBooking.Application.Bookings;
using RoomBooking.Application.Rooms;
using RoomBooking.Domain.Pricing;
using RoomBooking.Infrastructure.Persistence;
using RoomBooking.Infrastructure.Persistence.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddDbContext<RoomBookingDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();

builder.Services.AddScoped<RoomService>();
builder.Services.AddScoped<BookingService>();

builder.Services.AddSingleton(TariffSchedule.Default);
builder.Services.AddSingleton<RentalPriceCalculator>();

var app = builder.Build();

// Створення бази з міграцієй і початковими даними
if (app.Environment.IsDevelopment())
{
    using IServiceScope scope = app.Services.CreateScope();

    RoomBookingDbContext context = scope.ServiceProvider.GetRequiredService<RoomBookingDbContext>();

    await context.Database.MigrateAsync();
    await SeedData.EnsureSeededAsync(context);
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();