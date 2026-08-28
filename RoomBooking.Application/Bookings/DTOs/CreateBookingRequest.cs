namespace RoomBooking.Application.Bookings.DTOs
{

    public sealed record CreateBookingRequest(
        Guid RoomId,
        DateTimeOffset Start,
        DateTimeOffset End,
        IReadOnlyList<Guid> AmenityIds
        );
}