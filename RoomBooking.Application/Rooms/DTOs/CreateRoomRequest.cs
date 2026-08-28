namespace RoomBooking.Application.Rooms.DTOs
{
    public sealed record CreateRoomRequest(
        string Name,
        int Capacity,
        decimal PricePerHour,
        string TimeZoneId,
        IReadOnlyList<AmenityRequest> Amenities
        );
}
