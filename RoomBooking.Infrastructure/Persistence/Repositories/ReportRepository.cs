using Microsoft.EntityFrameworkCore;
using RoomBooking.Application.Reports;
using RoomBooking.Application.Reports.DTOs;
using RoomBooking.Domain.Shared;

namespace RoomBooking.Infrastructure.Persistence.Repositories
{
    public sealed class ReportRepository : IReportRepository
    {
        private readonly RoomBookingDbContext _context;

        public ReportRepository(RoomBookingDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<RoomReportItem>> GetRoomStatsAsync(
            TimeRange utcPeriod,
            CancellationToken cancellationToken)
        {
            var stats = await _context.Bookings
                .Where(booking => booking.Start < utcPeriod.End && utcPeriod.Start < booking.End)
                .GroupBy(booking => booking.RoomId)
                .Select(group => new
                {
                    RoomId = group.Key,
                    BookingsCount = group.Count(),
                    Revenue = group.Sum(booking => booking.TotalPrice)
                })
                .ToListAsync(cancellationToken);

            // IgnoreQueryFilters: видалений зал зник з каталогу, але гроші за ним отримані.
            Dictionary<Guid, string> names = await _context.Rooms
                .IgnoreQueryFilters()
                .ToDictionaryAsync(room => room.Id, room => room.Name, cancellationToken);

            return stats
                .Select(row => new RoomReportItem(
                    row.RoomId,
                    names[row.RoomId],
                    row.BookingsCount,
                    row.Revenue))
                .OrderByDescending(item => item.Revenue)
                .ToList();
        }

        public async Task<IReadOnlyList<DailyReportItem>> GetDailyStatsAsync(
            TimeRange utcPeriod,
            CancellationToken cancellationToken)
        {
            var stats = await _context.Bookings
                .Where(booking => booking.Start < utcPeriod.End && utcPeriod.Start < booking.End)
                .GroupBy(booking => booking.Start.Date)
                .Select(group => new
                {
                    Date = group.Key,
                    BookingsCount = group.Count(),
                    Revenue = group.Sum(booking => booking.TotalPrice)
                })
                .ToListAsync(cancellationToken);

            // DateOnly в SQL не перекладається, тому перетворюємо вже над результатом.
            return stats
                .Select(row => new DailyReportItem(
                    DateOnly.FromDateTime(row.Date),
                    row.BookingsCount,
                    row.Revenue))
                .OrderBy(item => item.Date)
                .ToList();
        }
    }
}