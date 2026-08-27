using RoomBooking.Domain.Exceptions;
using RoomBooking.Domain.Pricing;
using RoomBooking.Domain.Shared;

namespace RoomBooking.UnitTests.Pricing
{
    public class TariffScheduleTests
    {
        private static readonly DateTime Date = new(2024, 9, 1);

        private static TimeRange Booking(int startHour, int endHour)
        {
            return new TimeRange(Date.AddHours(startHour), Date.AddHours(endHour));
        }


        [Fact]
        public void Split_BookingWithinStandardHours_ReturnsSingleSegmentAtBaseRate()
        {
            var segments = TariffSchedule.Default.Split(Booking(9, 12));

            TariffSegment segment = Assert.Single(segments);

            Assert.Equal(TariffType.Standard, segment.Tariff.Type);
            Assert.Equal(1.00m, segment.Tariff.Multiplier);
        }

        [Fact]
        public void Split_BookingCrossingIntoEvening_CutsAtTheBoundary()
        {
            var segments = TariffSchedule.Default.Split(Booking(17, 19));

            Assert.Collection(
                segments,
                standard =>
                {
                    Assert.Equal(TariffType.Standard, standard.Tariff.Type);
                    Assert.Equal(TimeSpan.FromHours(1), standard.Range.Duration);
                },
                evening =>
                {
                    Assert.Equal(TariffType.Evening, evening.Tariff.Type);
                    Assert.Equal(0.80m, evening.Tariff.Multiplier);
                });
        }

        // Головний випадок: 12:00-14:00 належить і стандартному, і піковому інтервалу.
        [Fact]
        public void Split_BookingCoveringPeakHours_ChargesPeakInsteadOfStandard()
        {
            var segments = TariffSchedule.Default.Split(Booking(11, 15));

            Assert.Equal(
                new[] { TariffType.Standard, TariffType.Peak, TariffType.Standard },
                segments.Select(segment => segment.Tariff.Type));
        }


        [Fact]
        public void Split_BookingStartingBeforeNine_DiscountsOnlyTheEarlyPart()
        {
            var segments = TariffSchedule.Default.Split(Booking(7, 10));

            Assert.Collection(
                segments,
                morning => Assert.Equal(0.90m, morning.Tariff.Multiplier),
                standard => Assert.Equal(1.00m, standard.Tariff.Multiplier));
        }


        [Fact]
        public void Split_BookingReachingIntoTheNight_Throws()
        {
            Assert.Throws<BusinessRuleException>(() => TariffSchedule.Default.Split(Booking(22, 24)));
        }

    }



}
