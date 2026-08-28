namespace RoomBooking.Application.Bookings.DTOs
{
    public sealed record BookingResponse(
        Guid Id,
        Guid RoomId,
        DateTimeOffset Start,
        DateTimeOffset End,
        decimal RentalTotal,
        decimal AmenitiesTotal,
        decimal Total,
        IReadOnlyList<RentalChargeResponse> Charges
        );
}