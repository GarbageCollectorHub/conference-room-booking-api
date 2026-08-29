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

        public async Task<IReadOnlyList<Booking>> GetInPeriodAsync(
            TimeRange utcPeriod,
            CancellationToken cancellationToken)
        {
            return await _context.Bookings
                .Where(booking => booking.Start < utcPeriod.End && utcPeriod.Start < booking.End)
                .ToListAsync(cancellationToken);
        }

        // Serializable ізолює перевірку і вставку, щоб паралельна транзакція не створила
        // конфліктне бронювання, поки ця не завершилась.
        public async Task<bool> TryAddAsync(Booking booking, CancellationToken cancellationToken)
        {
            await using IDbContextTransaction transaction = await _context.Database
                .BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);

            try
            {
                if (await HasBookingAsync(booking.RoomId, booking.Period, cancellationToken))
                {
                    return false;
                }

                _context.Bookings.Add(booking);

                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                return true;
            }
            catch (Exception exception) when (IsDeadlock(exception))
            {
                // Транзакцію відкочено, і чи встиг конкурент зайняти час - достеменно невідомо.
                // Відповідаємо як на зайнятий зал,
                // Повніше рішення - повторити транзакцію.
                return false;
            }
        }

        private static bool IsDeadlock(Exception exception)
        {
            return exception is SqlException { Number: DeadlockErrorNumber }
                || exception.InnerException is SqlException { Number: DeadlockErrorNumber };
        }
    }
}