using RoomBooking.Domain.Shared;

namespace RoomBooking.UnitTests.Shared
{

    public class TimeRangeTests
    {
        private static readonly DateTime Date = new(2024, 9, 1);

        [Fact]
        public void Constructor_EndNotAfterStart_Throws()
        {
            Assert.Throws<ArgumentException>(() => new TimeRange(Date.AddHours(10), Date.AddHours(10)));
        }


        // Перевіряє порівняння діапазонів за значенням через Equals та "=="
        [Fact]
        public void Equality_TwoRangesWithSameBounds_AreEqual()
        {
            var first = new TimeRange(Date.AddHours(9), Date.AddHours(12));
            var second = new TimeRange(Date.AddHours(9), Date.AddHours(12));

            Assert.Equal(first, second);
        }
    }

}
