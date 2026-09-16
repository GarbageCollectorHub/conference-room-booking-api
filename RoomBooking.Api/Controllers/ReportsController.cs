using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoomBooking.Application.Reports;
using RoomBooking.Application.Reports.DTOs;
using RoomBooking.Domain.Shared;

namespace RoomBooking.Api.Controllers
{
    /// <summary>Business reports.</summary>
    [ApiController]
    [Authorize(Roles = "Admin")]
    [Route("api/reports")]
    public sealed class ReportsController : ControllerBase
    {
        private readonly IReportRepository _reports;

        public ReportsController(IReportRepository reports)
        {
            _reports = reports;
        }

        /// <summary>Bookings and revenue for each room.</summary>
        /// <remarks>From and To take time with offset, for example 2026-09-01T00:00:00+03:00.</remarks>
        [HttpGet("rooms")]
        public async Task<IReadOnlyList<RoomReportItem>> GetRooms(
            [FromQuery] ReportPeriodQuery query,
            CancellationToken cancellationToken)
        {
            return await _reports.GetRoomStatsAsync(new TimeRange(query.From.UtcDateTime, query.To.UtcDateTime), cancellationToken);
        }

        /// <summary>Bookings and revenue per day.</summary>
        /// <remarks>From and To take time with offset, for example 2026-09-01T00:00:00+03:00.</remarks>
        [HttpGet("daily")]
        public async Task<IReadOnlyList<DailyReportItem>> GetDaily(
            [FromQuery] ReportPeriodQuery query,
            CancellationToken cancellationToken)
        {
            return await _reports.GetDailyStatsAsync(new TimeRange(query.From.UtcDateTime, query.To.UtcDateTime), cancellationToken);
        }
    }
}