using RoomBooking.Application.Bookings.DTOs;
using RoomBooking.Application.Rooms;
using RoomBooking.Domain.Bookings;
using RoomBooking.Domain.Exceptions;
using RoomBooking.Domain.Pricing;
using RoomBooking.Domain.Rooms;
using RoomBooking.Domain.Shared;
using RoomBooking.Domain.Users;

namespace RoomBooking.Application.Bookings
{
    public sealed class BookingService
    {
        private readonly IRoomRepository _rooms;
        private readonly IBookingRepository _bookings;
        private readonly RentalPriceCalculator _calculator;

        public BookingService(
            IBookingRepository bookings,
            IRoomRepository rooms,
            RentalPriceCalculator calculator)
        {
            _bookings = bookings;
            _rooms = rooms;
            _calculator = calculator;
        }


        public async Task<BookingResponse> CreateAsync(
            CreateBookingRequest request,
            Guid userId,
            CancellationToken cancellationToken
            )
        {
            Room? room = await _rooms.GetByIdAsync(request.RoomId, cancellationToken);

            if (room is null)
            {
                throw new NotFoundException($"Room {request.RoomId} was not found.");
            }

            TimeRange utcPeriod = new(request.Start.UtcDateTime, request.End.UtcDateTime);

            // Рання перевірка відсікає звичайний випадок, не витрачаючи час на розрахунок.
            if (await _bookings.HasBookingAsync(room.Id, utcPeriod, cancellationToken))
            {
                throw new ConflictException("Room is already booked for this time.");
            }

            TimeRange localPeriod = new(
                room.ToLocalTime(utcPeriod.Start),
                room.ToLocalTime(utcPeriod.End));


            IReadOnlyList<Amenity> amenities = room.GetAmenities(request.AmenityIds);
            decimal amenitiesTotal = amenities.Sum(amenity => amenity.Price);

            BookingPrice price = _calculator.Calculate(room.PricePerHour, localPeriod, amenitiesTotal);

            Booking booking = new(room, userId, localPeriod, amenities, price);

            // Транзакція проти race condition
            if (!await _bookings.TryAddAsync(booking, cancellationToken))
            {
                throw new ConflictException("Room is already booked for this time.");
            }

            return booking.ToResponse(price);

        }

    }
}
