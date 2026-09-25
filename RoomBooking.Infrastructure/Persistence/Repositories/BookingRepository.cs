using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using RoomBooking.Application.Bookings;
using RoomBooking.Domain.Bookings;
using RoomBooking.Domain.Shared;

namespace RoomBooking.Infrastructure.Persistence.Repositories
{
    public sealed class BookingRepository : IBookingRepository
    {
        // Номер помилки SQL Server для взаємного блокування
        private const int DeadlockErrorNumber = 1205;

        private readonly RoomBookingDbContext _context;

        public BookingRepository(RoomBookingDbContext context)
        {
            _context = context;
        }


        public async Task<bool> HasBookingAsync(
            Guid roomId,
            TimeRange utcPeriod,
            CancellationToken cancellationToken)
        {
            return await _context.Bookings.AnyAsync(
                booking => booking.RoomId == roomId
                    && booking.Start < utcPeriod.End
                    && utcPeriod.Start < booking.End,
                cancellationToken);
        }

        public async Task<bool> HasFutureBookingsAsync(Guid roomId, CancellationToken cancellationToken)
        {
            DateTime now = DateTime.UtcNow;

            return await _context.Bookings.AnyAsync(
                booking => booking.RoomId == roomId && booking.End > now,
                cancellationToken);
        }


        // Блокуємо рядок залу до кінця транзакції: конкурентні бронювання одного залу очікують одне одного,
        // а для різних залів можуть виконуватися паралельно.
        public async Task<bool> TryAddAsync(Booking booking, CancellationToken cancellationToken)
        {
            await using IDbContextTransaction transaction = await _context.Database
                .BeginTransactionAsync(cancellationToken);

            // UPDLOCK тримається до Commit або Rollback. Другий запит на цей зал чекає тут,
            // звичайні читання залу (GET) не блокуються. Синтаксис SQL Server.
            await _context.Database.ExecuteSqlAsync(
                $"SELECT Id FROM Rooms WITH (UPDLOCK, ROWLOCK) WHERE Id = {booking.RoomId}",
                cancellationToken);

            // Перевіряємо бронювання після отримання доступу до кімнати через UPDLOCK, щоб уникнути race condition
            if (await HasBookingAsync(booking.RoomId, booking.Period, cancellationToken))
            {
                return false;
            }

            _context.Bookings.Add(booking);

            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return true;
        }

    }
}