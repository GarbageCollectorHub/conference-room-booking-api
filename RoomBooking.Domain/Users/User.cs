using RoomBooking.Domain.Exceptions;

namespace RoomBooking.Domain.Users
{
    public sealed class User
    {
        public const int EmailMaxLength = 256;

        public Guid Id { get; private set; }
        public string Email { get; private set; } = string.Empty;
        public string PasswordHash { get; private set; } = string.Empty;
        public UserRole Role { get; private set; }


        public User(string email, string passwordHash, UserRole role)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new BusinessRuleException("Email is required.");
            }

            if (email.Length > EmailMaxLength)
            {
                throw new BusinessRuleException($"Email must be at most {EmailMaxLength} characters.");
            }

            Id = Guid.NewGuid();
            Email = email.Trim().ToLowerInvariant();
            PasswordHash = passwordHash;
            Role = role;
        }

        private User() { }

    }
}