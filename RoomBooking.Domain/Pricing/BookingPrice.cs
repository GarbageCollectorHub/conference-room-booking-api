namespace RoomBooking.Domain.Pricing
{

    // Ціна одного бронювання, розкладена на частини.
    // Підсумки не зберігаються окремо, а рахуються від складових - інакше вони можуть розійтися.

    public sealed class BookingPrice
    {
        public IReadOnlyList<RentalCharge> RentalCharges { get; }
        public decimal RentalTotal { get; }
        public decimal AmenitiesTotal { get; }
        public decimal Total { get; }

        public BookingPrice(IReadOnlyList<RentalCharge> rentalCharges, decimal amenitiesTotal)
        {
            RentalCharges = rentalCharges;
            RentalTotal = rentalCharges.Sum(charge => charge.Cost);
            AmenitiesTotal = amenitiesTotal;
            Total = RentalTotal + amenitiesTotal;
        }

    }
}