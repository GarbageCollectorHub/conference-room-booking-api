using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoomBooking.Application.Rooms;
using RoomBooking.Application.Rooms.DTOs;

namespace RoomBooking.Api.Controllers
{
    /// <summary>Conference rooms.</summary>
    [ApiController]
    [Route("api/rooms")]
    public sealed class RoomsController : ControllerBase
    {
        private readonly RoomService _rooms;

        public RoomsController(RoomService rooms)
        {
            _rooms = rooms;
        }

        /// <summary>Returns the full room catalog.</summary>
        /// <remarks>
        /// Lets you see the data and copy room or amenity ids when testing in Swagger.
        /// Rooms are few, so the list is not paged.
        /// </remarks>
        [HttpGet]
        public async Task<IReadOnlyList<RoomResponse>> GetAll(CancellationToken cancellationToken)
        {
            return await _rooms.GetAllAsync(cancellationToken);
        }


        /// <summary>Returns one room by id.</summary>
        [HttpGet("{id:guid}")]
        public async Task<RoomResponse> GetById(Guid id, CancellationToken cancellationToken)
        {
            return await _rooms.GetByIdAsync(id, cancellationToken);
        }


        /// <summary>Returns rooms that are free for the given time.</summary>
        /// <param name="start">Start time with offset, for example 2026-09-01T10:00:00+03:00</param>
        /// <param name="end">End time in the same format.</param>
        /// <param name="capacity">How many people the room must fit.</param>
        /// <param name="cancellationToken">Cancels the request</param>
        [HttpGet("available")]
        public async Task<IReadOnlyList<RoomResponse>> GetAvailable(
            [FromQuery] DateTimeOffset start,
            [FromQuery] DateTimeOffset end,
            [FromQuery] int capacity,
            CancellationToken cancellationToken)
        {
            return await _rooms.FindAvailableAsync(start, end, capacity, cancellationToken);
        }


        /// <summary>Creates a room with its amenities.</summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<RoomResponse>> Create(
            CreateRoomRequest request,
            CancellationToken cancellationToken)
        {
            RoomResponse room = await _rooms.CreateAsync(request, cancellationToken);

            return CreatedAtAction(nameof(GetById), new { id = room.Id }, room);
        }


        /// <summary>Updates name, capacity, price per hour or time zone.</summary>
        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<RoomResponse> Update(
            Guid id,
            UpdateRoomRequest request,
            CancellationToken cancellationToken)
        {
            return await _rooms.UpdateAsync(id, request, cancellationToken);
        }


        /// <summary>Adds one amenity to the room.</summary>
        [HttpPost("{id:guid}/amenities")]
        [Authorize(Roles = "Admin")]
        public async Task<RoomResponse> AddAmenity(
            Guid id,
            AmenityRequest request,
            CancellationToken cancellationToken)
        {
            return await _rooms.AddAmenityAsync(id, request, cancellationToken);
        }


        /// <summary>Deletes a room. A room with future bookings cannot be deleted.</summary>
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            await _rooms.DeleteAsync(id, cancellationToken);

            return NoContent();
        }
    }
}