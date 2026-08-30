using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RoomBooking.Domain.Users;

namespace RoomBooking.Infrastructure.Persistence.Configurations
{
    public sealed class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(user => user.Id);

            builder.Property(user => user.Id).ValueGeneratedNever();

            builder.Property(user => user.Email)
                .IsRequired()
                .HasMaxLength(User.EmailMaxLength);

            builder.HasIndex(user => user.Email)
                .IsUnique();

            builder.Property(user => user.PasswordHash)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(user => user.Role)
                .HasConversion<string>()
                .HasMaxLength(20);

        }
    }
}