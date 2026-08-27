using RoomBooking.Domain.Pricing;
using RoomBooking.Domain.Shared;

namespace RoomBooking.UnitTests.Pricing
{
    public class RentalPriceCalculatorTests
    {
        private const decimal PricePerHour = 2000m;

        private static readonly DateTime Date = new(2024, 9, 1);

        private readonly RentalPriceCalculator _calculator = new(TariffSchedule.Default);


        private static TimeRange Booking(int startHour, int endHour)
        {
            return new TimeRange(Date.AddHours(startHour), Date.AddHours(endHour));
        }


        [Fact]
        public void Calculate_StandardHours_ChargesBaseRate()
        {
            BookingPrice price = _calculator.Calculate(PricePerHour, Booking(9, 12), 0m);

            Assert.Equal(6000m, price.Total);
        }


        // 17:00-18:00 за базовою ставкою, 18:00-19:00 зі знижкою 20%
        [Fact]
        public void Calculate_BookingCrossingIntoEvening_ChargesEachPartSeparately()
        {
            BookingPrice price = _calculator.Calculate(PricePerHour, Booking(17, 19), 0m);

            Assert.Equal(2, price.RentalCharges.Count);
            Assert.Equal(3600m, price.Total);
        }


        // 11:00-12:00 базова, 12:00-14:00 з націнкою 15%, 14:00-15:00 базова
        [Fact]
        public void Calculate_BookingCoveringPeakHours_AddsSurchargeOnlyToPeakPart()
        {
            BookingPrice price = _calculator.Calculate(PricePerHour, Booking(11, 15), 0m);

            Assert.Equal(8600m, price.Total);
        }


        // Знижка діє тільки на оренду залу, послуги додаються повною ціною
        [Fact]
        public void Calculate_WithAmenities_AddsThemToTotalWithoutDiscount()
        {
            BookingPrice price = _calculator.Calculate(PricePerHour, Booking(18, 19), 800m);

            Assert.Equal(1600m, price.RentalTotal);
            Assert.Equal(2400m, price.Total);
        }
    }
}