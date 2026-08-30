using Microsoft.EntityFrameworkCore;
using RoomBooking.Application.Rooms;
using RoomBooking.Domain.Rooms;
using RoomBooking.Domain.Shared;

namespace RoomBooking.Infrastructure.Persistence.Repositories
{
    public sealed class RoomRepository : IRoomRepository
    {

        private readonly RoomBookingDbContext _context;

        public RoomRepository(RoomBookingDbContext context)
        {
            _context = context;
        }


        public async Task<Room?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.Rooms
                .Include(room => room.Amenities)
                .FirstOrDefaultAsync(room => room.Id == id, cancellationToken);
        }

        public async Task<IReadOnlyList<Room>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _context.Rooms
                .AsNoTracking()
                .Include(room => room.Amenities)
                .OrderBy(room => room.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Room>> GetAvailableAsync(TimeRange utcPeriod, int capacity,
            CancellationToken cancellationToken)
        {
            return await _context.Rooms
                .AsNoTracking()
                .Include(room => room.Amenities)
                .Where(room => room.Capacity >= capacity)
                .Where(room => !_context.Bookings.Any(booking =>
                    booking.RoomId == room.Id
                    && booking.Start < utcPeriod.End
                    && utcPeriod.Start < booking.End))
                .OrderBy(room => room.Name)
                .ToListAsync(cancellationToken);
        }

        public void Add(Room room)
        {
            _context.Rooms.Add(room);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }


        public async Task<bool> NameTakenAsync(string name, Guid? exceptRoomId,
            CancellationToken cancellationToken)
        {
            return await _context.Rooms.AnyAsync(
                room => room.Name == name && (exceptRoomId == null || room.Id != exceptRoomId),
                cancellationToken);
        }

    }
}