using RoomBooking.Application.Users;

namespace RoomBooking.Infrastructure.Security
{
    // BCrypt сам генерує сіль і зберігає її разом з хешем

    public sealed class PasswordHasher : IPasswordHasher
    {
        public string Hash(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool Verify(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
    }
}