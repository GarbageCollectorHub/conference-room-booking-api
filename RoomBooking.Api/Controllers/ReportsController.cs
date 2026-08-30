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
        /// <param name="from">Period start with offset, for example 2026-09-01T00:00:00+03:00</param>
        /// <param name="to">Period end in the same format.</param>
        /// <param name="cancellationToken">Cancels the request.</param>
        [HttpGet("rooms")]
        public async Task<IReadOnlyList<RoomReportItem>> GetRooms(
            [FromQuery] DateTimeOffset from,
            [FromQuery] DateTimeOffset to,
            CancellationToken cancellationToken)
        {
            return await _reports.GetRoomStatsAsync(new TimeRange(from.UtcDateTime, to.UtcDateTime), cancellationToken);
        }

        /// <summary>Bookings and revenue per day.</summary>
        /// <param name="from">Period start with offset, for example 2026-09-01T00:00:00+03:00</param>
        /// <param name="to">Period end in the same format.</param>
        /// <param name="cancellationToken">Cancels the request.</param>
        [HttpGet("daily")]
        public async Task<IReadOnlyList<DailyReportItem>> GetDaily(
            [FromQuery] DateTimeOffset from,
            [FromQuery] DateTimeOffset to,
            CancellationToken cancellationToken)
        {
            return await _reports.GetDailyStatsAsync(new TimeRange(from.UtcDateTime, to.UtcDateTime), cancellationToken);
        }
    }
}