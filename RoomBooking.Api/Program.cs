using Microsoft.EntityFrameworkCore;
using RoomBooking.Api.ErrorHandling;
using RoomBooking.Application.Bookings;
using RoomBooking.Application.Rooms;
using RoomBooking.Application.Users;
using RoomBooking.Domain.Pricing;
using RoomBooking.Infrastructure.Persistence;
using RoomBooking.Infrastructure.Persistence.Repositories;
using RoomBooking.Infrastructure.Security;
using Swashbuckle.AspNetCore.Filters;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddSwaggerGen(options =>
{
    string xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";

    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFile));
    options.ExampleFilters();
});

builder.Services.AddSwaggerExamplesFromAssemblyOf<Program>();

builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Extensions["traceId"] = context.HttpContext.TraceIdentifier;
    };
});


builder.Services.AddExceptionHandler<DomainExceptionHandler>();

builder.Services.AddDbContext<RoomBookingDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));


builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();

builder.Services.AddScoped<RoomService>();
builder.Services.AddScoped<BookingService>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<AuthService>();

builder.Services.AddScoped<SeedData>();

builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();
builder.Services.AddSingleton<ITokenService, JwtTokenService>();

builder.Services.AddSingleton(TariffSchedule.Default);
builder.Services.AddSingleton<RentalPriceCalculator>();


var app = builder.Build();

// Створення бази з міграцій і початкові дані
if (app.Environment.IsDevelopment())
{
    using IServiceScope scope = app.Services.CreateScope();
    RoomBookingDbContext context = scope.ServiceProvider.GetRequiredService<RoomBookingDbContext>();

    await context.Database.MigrateAsync();
    await scope.ServiceProvider.GetRequiredService<SeedData>().SeedAsync();
}

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();