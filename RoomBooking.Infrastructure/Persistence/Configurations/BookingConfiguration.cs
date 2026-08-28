using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RoomBooking.Domain.Bookings;
using RoomBooking.Domain.Rooms;
using RoomBooking.Domain.Users;

namespace RoomBooking.Infrastructure.Persistence.Configurations
{
    public sealed class BookingConfiguration : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> builder)
        {
            builder.HasKey(booking => booking.Id);

            builder.HasOne<Room>()
                .WithMany()
                .HasForeignKey(booking => booking.RoomId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(booking => booking.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(booking => booking.TotalPrice)
                .HasPrecision(18, 2);

            // Period — Value Object, що обчислюється з Start і End, тому окремо не зберігається в БД.
            builder.Ignore(booking => booking.Period);

            // Головний запит системи - чи вільний зал у цей проміжок.
            builder.HasIndex(booking => new { booking.RoomId, booking.Start, booking.End });

            builder.HasMany(booking => booking.Amenities)
                .WithMany();

            builder.Metadata
                .FindSkipNavigation(nameof(Booking.Amenities))!
                .SetPropertyAccessMode(PropertyAccessMode.Field);

        }
    }
}