using RoomBooking.Domain.Rooms;
using RoomBooking.Domain.Shared;

namespace RoomBooking.Application.Rooms
{
    public interface IRoomRepository
    {

        void Add(Room room);

        Task SaveChangesAsync(CancellationToken cancellationToken);

        Task<Room?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

        Task<IReadOnlyList<Room>> GetAllAsync(CancellationToken cancellationToken);

        Task<IReadOnlyList<Room>> GetAvailableAsync(
            TimeRange utcPeriod,
            int capacity,
            CancellationToken cancellationToken
            );

    }

}
