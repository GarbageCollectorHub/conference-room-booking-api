using Microsoft.EntityFrameworkCore;
using RoomBooking.Domain.Bookings;
using RoomBooking.Domain.Rooms;
using RoomBooking.Domain.Users;

namespace RoomBooking.Infrastructure.Persistence
{
    public sealed class RoomBookingDbContext : DbContext
    {
        public RoomBookingDbContext(DbContextOptions<RoomBookingDbContext> options)
            : base(options)
        {
        }

        public DbSet<Room> Rooms => Set<Room>();
        public DbSet<Amenity> Amenities => Set<Amenity>();
        public DbSet<Booking> Bookings => Set<Booking>();
        public DbSet<User> Users => Set<User>();


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(RoomBookingDbContext).Assembly);
        }
    }
}