using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RoomBooking.Domain.Rooms;

namespace RoomBooking.Infrastructure.Persistence.Configurations
{
    public sealed class AmenityConfiguration : IEntityTypeConfiguration<Amenity>
    {
        public void Configure(EntityTypeBuilder<Amenity> builder)
        {
            builder.HasKey(amenity => amenity.Id);

            builder.Property(amenity => amenity.Id).ValueGeneratedNever();

            builder.Property(amenity => amenity.Name)
                .IsRequired()
                .HasMaxLength(Amenity.NameMaxLength);

            builder.Property(amenity => amenity.Price)
                .HasPrecision(18, 2);

        }
    }
}