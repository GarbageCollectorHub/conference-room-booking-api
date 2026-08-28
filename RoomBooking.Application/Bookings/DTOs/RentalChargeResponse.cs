namespace RoomBooking.Application.Bookings.DTOs
{

    public sealed record RentalChargeResponse(
        string Tariff,
        DateTime LocalStart,
        DateTime LocalEnd,
        decimal Cost
        );
}