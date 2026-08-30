using Microsoft.EntityFrameworkCore;
using RoomBooking.Api;
using RoomBooking.Api.ErrorHandling;
using RoomBooking.Application.Bookings;
using RoomBooking.Application.Reports;
using RoomBooking.Application.Rooms;
using RoomBooking.Application.Users;
using RoomBooking.Domain.Pricing;
using RoomBooking.Infrastructure.Persistence;
using RoomBooking.Infrastructure.Persistence.Repositories;
using RoomBooking.Infrastructure.Security;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddApiDocumentation();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddValidationResponses();


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
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<RoomService>();
builder.Services.AddScoped<BookingService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<SeedData>();

builder.Services.AddRequestRateLimiting();

builder.Services.AddSingleton(TariffSchedule.Default);
builder.Services.AddSingleton<RentalPriceCalculator>();
builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();
builder.Services.AddSingleton<ITokenService, JwtTokenService>();


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
app.UseStatusCodePages();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();

app.MapControllers();

app.Run();