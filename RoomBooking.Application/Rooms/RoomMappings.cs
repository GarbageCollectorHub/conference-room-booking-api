using RoomBooking.Application.Rooms.DTOs;
using RoomBooking.Domain.Rooms;

namespace RoomBooking.Application.Rooms
{
    // Перетворення доменних обʼєктів у DTO. Винесено окремо, щоб форму відповіді
    // можна було міняти в одному місці, а домен не знав про контракти API.

    public static class RoomMappings
    {
        public static RoomResponse ToResponse(this Room room)
        {
            List<AmenityResponse> amenities = room.Amenities
                .Select(amenity => amenity.ToResponse())
                .ToList();

            return new RoomResponse(
                room.Id,
                room.Name,
                room.Capacity,
                room.PricePerHour,
                room.TimeZoneId,
                amenities);
        }

        public static AmenityResponse ToResponse(this Amenity amenity)
        {
            return new AmenityResponse(amenity.Id, amenity.Name, amenity.Price);
        }

    }
}