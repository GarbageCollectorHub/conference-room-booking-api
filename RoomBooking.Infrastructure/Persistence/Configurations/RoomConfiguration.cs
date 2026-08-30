using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RoomBooking.Domain.Rooms;

namespace RoomBooking.Infrastructure.Persistence.Configurations
{
    public sealed class RoomConfiguration : IEntityTypeConfiguration<Room>
    {
        public void Configure(EntityTypeBuilder<Room> builder)
        {
            builder.HasKey(room => room.Id);

            builder.Property(room => room.Id).ValueGeneratedNever();

            builder.Property(room => room.Name)
                .IsRequired()
                .HasMaxLength(Room.NameMaxLength);

            builder.Property(room => room.TimeZoneId)
                .IsRequired()
                .HasMaxLength(64);

            builder.Property(room => room.PricePerHour)
                .HasPrecision(18, 2);

            builder.HasMany(room => room.Amenities)
                .WithOne()
                .HasForeignKey("RoomId")
                .OnDelete(DeleteBehavior.Cascade);

            // Змушуємо EF Core працювати з backing field _amenities,
            // а не з property Amenities
            builder.Metadata
                .FindNavigation(nameof(Room.Amenities))!
                .SetPropertyAccessMode(PropertyAccessMode.Field);

            // Видалені зали не потрапляють у вибірки, але лишаються в базі заради історії.
            builder.HasQueryFilter(room => !room.IsDeleted);
        }
    }
}