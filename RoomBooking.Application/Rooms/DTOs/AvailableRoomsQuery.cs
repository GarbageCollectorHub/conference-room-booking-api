


namespace RoomBooking.Application.Rooms.DTOs
{
    public sealed record AvailableRoomsQuery(DateTimeOffset Start, DateTimeOffset End, int Capacity);
}