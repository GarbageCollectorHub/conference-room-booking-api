using RoomBooking.Domain.Shared;

namespace RoomBooking.Domain.Pricing
{
    public sealed class RentalPriceCalculator
    {
        private const int MoneyDecimals = 2;

        private readonly TariffSchedule _schedule;

        public RentalPriceCalculator(TariffSchedule schedule)
        {
            _schedule = schedule;
        }

        public BookingPrice Calculate(decimal pricePerHour, TimeRange booking, decimal amenitiesTotal)
        {
            List<RentalCharge> charges = new();

            foreach (TariffSegment segment in _schedule.Split(booking))
            {
                decimal hours = (decimal)segment.Range.Duration.TotalHours;
                decimal cost = Round(pricePerHour * segment.Tariff.Multiplier * hours);

                charges.Add(new RentalCharge(segment.Tariff.Type, segment.Range, cost));
            }

            return new BookingPrice(charges, Round(amenitiesTotal));
        }

        // Округляємо кожен рядок окремо, а підсумок рахуємо як суму округлених рядків.
        // Інакше в чеку сума рядків не дорівнюватиме підсумку.

        private static decimal Round(decimal value)
        {
            return Math.Round(value, MoneyDecimals, MidpointRounding.AwayFromZero);
        }

    }
}