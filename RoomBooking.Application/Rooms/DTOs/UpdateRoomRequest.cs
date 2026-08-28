namespace RoomBooking.Application.Rooms.DTOs
{
    public sealed record UpdateRoomRequest(
        string Name,
        int Capacity,
        decimal PricePerHour,
        string TimeZoneId
        );
}
