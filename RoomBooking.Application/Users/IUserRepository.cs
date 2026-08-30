using RoomBooking.Domain.Users;

namespace RoomBooking.Application.Users
{
    public interface IUserRepository
    {
        Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken);

        Task<bool> ExistsAsync(string email, CancellationToken cancellationToken);

        void Add(User user);

        Task SaveChangesAsync(CancellationToken cancellationToken);
    }

}