using RoomBooking.Application.Bookings.DTOs;
using RoomBooking.Domain.Bookings;
using RoomBooking.Domain.Pricing;

namespace RoomBooking.Application.Bookings
{
    public static class BookingMappings
    {
        // У Booking зберігається лише підсумок, тому розбивка приходить окремим параметром.
        public static BookingResponse ToResponse(this Booking booking, BookingPrice price)
        {
            List<RentalChargeResponse> charges = price.RentalCharges
                .Select(charge => charge.ToResponse())
                .ToList();

            return new BookingResponse(
                booking.Id,
                booking.RoomId,
                new DateTimeOffset(booking.Start, TimeSpan.Zero),
                new DateTimeOffset(booking.End, TimeSpan.Zero),
                price.RentalTotal,
                price.AmenitiesTotal,
                booking.TotalPrice,
                charges
                );
        }

        public static RentalChargeResponse ToResponse(this RentalCharge charge)
        {
            return new RentalChargeResponse(
                charge.Tariff.ToString(),
                charge.Range.Start,
                charge.Range.End,
                charge.Cost
                );
        }

    }
}