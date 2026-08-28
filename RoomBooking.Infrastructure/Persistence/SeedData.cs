using Microsoft.EntityFrameworkCore;
using RoomBooking.Domain.Rooms;

namespace RoomBooking.Infrastructure.Persistence
{
    // Початкові дані, щоб застосунок був придатний до роботи одразу після запуску.
    public static class SeedData
    {
        private const string DefaultTimeZoneId = "Europe/Kyiv";

        public static async Task EnsureSeededAsync(
            RoomBookingDbContext context,
            CancellationToken cancellationToken = default
            )
        {
            if (await context.Rooms.AnyAsync(cancellationToken))
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

            context.Rooms.AddRange(hallA, hallB, hallC);

            await context.SaveChangesAsync(cancellationToken);


        }
  
    }
}