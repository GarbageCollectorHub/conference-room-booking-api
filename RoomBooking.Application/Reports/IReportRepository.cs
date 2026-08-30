using RoomBooking.Application.Reports.DTOs;
using RoomBooking.Domain.Shared;

namespace RoomBooking.Application.Reports
{

    public interface IReportRepository
    {
        Task<IReadOnlyList<RoomReportItem>> GetRoomStatsAsync(
            TimeRange utcPeriod,
            CancellationToken cancellationToken);

        Task<IReadOnlyList<DailyReportItem>> GetDailyStatsAsync(
            TimeRange utcPeriod,
            CancellationToken cancellationToken);

    }
}