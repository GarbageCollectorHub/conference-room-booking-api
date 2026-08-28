using RoomBooking.Application.Bookings;
using RoomBooking.Application.Rooms.DTOs;
using RoomBooking.Domain.Bookings;
using RoomBooking.Domain.Exceptions;
using RoomBooking.Domain.Rooms;
using RoomBooking.Domain.Shared;

namespace RoomBooking.Application.Rooms
{
    public sealed class RoomService
    {
        private readonly IRoomRepository _rooms;
        private readonly IBookingRepository _bookings;

        public RoomService(IRoomRepository rooms, IBookingRepository bookings)
        {
            _rooms = rooms;
            _bookings = bookings;
        }


        public async Task<RoomResponse> CreateAsync(CreateRoomRequest request, CancellationToken cancellationToken)
        {
            Room room = new(request.Name, request.Capacity, request.PricePerHour, request.TimeZoneId);

            foreach (AmenityRequest amenity in request.Amenities)
            {
                room.AddAmenity(new Amenity(amenity.Name, amenity.Price));
            }

            _rooms.Add(room);
            await _rooms.SaveChangesAsync(cancellationToken);

            return room.ToResponse();
        }

        public async Task<RoomResponse> UpdateAsync(
            Guid id,
            UpdateRoomRequest request,
            CancellationToken cancellationToken
            )
        {
            Room room = await GetRequiredAsync(id, cancellationToken);

            room.Rename(request.Name);
            room.ChangeCapacity(request.Capacity);
            room.ChangePricePerHour(request.PricePerHour);
            room.ChangeTimeZone(request.TimeZoneId);

            await _rooms.SaveChangesAsync(cancellationToken);

            return room.ToResponse();
        }

        public async Task<RoomResponse> AddAmenityAsync(
            Guid id,
            AmenityRequest request,
            CancellationToken cancellationToken
            )
        {
            Room room = await GetRequiredAsync(id, cancellationToken);

            room.AddAmenity(new Amenity(request.Name, request.Price));

            await _rooms.SaveChangesAsync(cancellationToken);

            return room.ToResponse();
        }


        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            Room room = await GetRequiredAsync(id, cancellationToken);

            if (await _bookings.HasFutureBookingsAsync(id, cancellationToken))
            {
                throw new ConflictException("Room has upcoming bookings and cannot be deleted.");
            }

            room.MarkDeleted();
            await _rooms.SaveChangesAsync(cancellationToken);
        }


        public async Task<IReadOnlyList<RoomResponse>> GetAllAsync(CancellationToken cancellationToken)
        {
            IReadOnlyList<Room> rooms = await _rooms.GetAllAsync(cancellationToken);

            return rooms.Select(room => room.ToResponse()).ToList();
        }


        public async Task<IReadOnlyList<RoomResponse>> FindAvailableAsync(
           DateTimeOffset start,
           DateTimeOffset end,
           int capacity,
           CancellationToken cancellationToken
            )
        {
            TimeRange utcPeriod = new(start.UtcDateTime, end.UtcDateTime);

            IReadOnlyList<Room> rooms = await _rooms.GetAvailableAsync(utcPeriod, capacity, cancellationToken);

            return rooms.Select(room => room.ToResponse()).ToList();
        }

        private async Task<Room> GetRequiredAsync(Guid id, CancellationToken cancellationToken)
        {
            Room? room = await _rooms.GetByIdAsync(id, cancellationToken);

            if (room is null)
            {
                throw new NotFoundException($"Room {id} was not found.");
            }

            return room;
        }

    }
}
