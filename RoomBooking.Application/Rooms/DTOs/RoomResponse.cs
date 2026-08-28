namespace RoomBooking.Application.Rooms.DTOs
{
    public sealed record RoomResponse(
        Guid Id,
        string Name,
        int Capacity,
        decimal PricePerHour,
        string TimeZoneId,
        IReadOnlyList<AmenityResponse> Amenities
        );
}
