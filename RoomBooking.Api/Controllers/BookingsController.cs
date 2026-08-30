using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoomBooking.Application.Bookings;
using RoomBooking.Application.Bookings.DTOs;

namespace RoomBooking.Api.Controllers
{
    /// <summary>Room bookings</summary>
    [ApiController]
    [Authorize]
    [Route("api/bookings")]
    public sealed class BookingsController : ControllerBase
    {
        private readonly BookingService _bookings;

        public BookingsController(BookingService bookings)
        {
            _bookings = bookings;
        }

        /// <summary>Books a room and returns the total price with a breakdown by tariff</summary>
        [HttpPost]
        public async Task<BookingResponse> Create(
            CreateBookingRequest request,
            CancellationToken cancellationToken)
        {
            return await _bookings.CreateAsync(request, GetUserId(), cancellationToken);
        }

        // Беремо користувача iз токена
        private Guid GetUserId()
        {
            return Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }

    }
}