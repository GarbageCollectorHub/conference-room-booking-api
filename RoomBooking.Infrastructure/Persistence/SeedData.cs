using Microsoft.EntityFrameworkCore;
using RoomBooking.Application.Users;
using RoomBooking.Domain.Rooms;
using RoomBooking.Domain.Users;

namespace RoomBooking.Infrastructure.Persistence
{
    // Початкові дані, щоб застосунок був придатний до роботи одразу після запуску.
    public sealed class SeedData
    {
        private const string DefaultTimeZoneId = "Europe/Kyiv";

        private readonly RoomBookingDbContext _context;
        private readonly IPasswordHasher _passwordHasher;

        public SeedData(RoomBookingDbContext context, IPasswordHasher passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        public async Task SeedAsync(CancellationToken cancellationToken = default)
        {
            await SeedRoomsAsync(cancellationToken);
            await SeedUsersAsync(cancellationToken);
        }

        private async Task SeedRoomsAsync(CancellationToken cancellationToken)
        {
            if (await _context.Rooms.AnyAsync(cancellationToken))
            {
                return;
            }

            Room hallA = new("Hall A", 50, 2000m, DefaultTimeZoneId);
            hallA.AddAmenity(new Amenity("Projector", 500m));
            hallA.AddAmenity(new Amenity("Wi-Fi", 300m));
            hallA.AddAmenity(new Amenity("Sound system", 700m));

            Room hallB = new("Hall B", 100, 3500m, DefaultTimeZoneId);
            hallB.AddAmenity(new Amenity("Projector", 500m));
            hallB.AddAmenity(new Amenity("Wi-Fi", 300m));
            hallB.AddAmenity(new Amenity("Sound system", 700m));

            Room hallC = new("Hall C", 30, 1500m, DefaultTimeZoneId);
            hallC.AddAmenity(new Amenity("Wi-Fi", 300m));

            _context.Rooms.AddRange(hallA, hallB, hallC);

            await _context.SaveChangesAsync(cancellationToken);
        }

        private async Task SeedUsersAsync(CancellationToken cancellationToken)
        {
            if (await _context.Users.AnyAsync(cancellationToken))
            {
                return;
            }

            User admin = new("admin@example.com", _passwordHasher.Hash("admin"), UserRole.Admin);
            User client = new("tester@example.com", _passwordHasher.Hash("tester"), UserRole.Client);

            _context.Users.AddRange(admin, client);

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}