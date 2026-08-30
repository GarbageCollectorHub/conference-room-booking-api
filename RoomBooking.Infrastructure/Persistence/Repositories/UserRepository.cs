using Microsoft.EntityFrameworkCore;
using RoomBooking.Application.Users;
using RoomBooking.Domain.Users;

namespace RoomBooking.Infrastructure.Persistence.Repositories
{
    public sealed class UserRepository : IUserRepository
    {
        private readonly RoomBookingDbContext _context;

        public UserRepository(RoomBookingDbContext context)
        {
            _context = context;
        }


        public async Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken)
        {
            string normalized = User.NormalizeEmail(email);

            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(user => user.Email == normalized, cancellationToken);
        }

        public async Task<bool> ExistsAsync(string email, CancellationToken cancellationToken)
        {
            string normalized = User.NormalizeEmail(email);

            return await _context.Users.AnyAsync(user => user.Email == normalized, cancellationToken);
        }

        public void Add(User user)
        {
            _context.Users.Add(user);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}