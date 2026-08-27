using RoomBooking.Domain.Exceptions;
using RoomBooking.Domain.Shared;

namespace RoomBooking.Domain.Pricing
{

    // тарифні періоди дня і розрізання бронювання на їхні частини

    public class TariffSchedule
    {
        private readonly IReadOnlyList<TariffPeriod> _periods;


        // Тарифи за умовами задачі. Multiplier - множник до базової ставки за годину:
        // 0.90 це знижка 10%, 1.15 це націнка 15%.
        // Пікові години лежать усередині стандартних, тому саме цей період має Priority 1:
        // о 12:00-14:00 нараховується піковий тариф, а не стандартний.
        // Час з 23:00 до 06:00 не описаний жодним періодом - бронювати його не можна.
        public static TariffSchedule Default { get; } = new(new[]
        {
            new TariffPeriod(TariffType.Morning,  new TimeOnly(6, 0),  new TimeOnly(9, 0),  0.90m),
            new TariffPeriod(TariffType.Standard, new TimeOnly(9, 0),  new TimeOnly(18, 0), 1.00m),
            new TariffPeriod(TariffType.Peak,     new TimeOnly(12, 0), new TimeOnly(14, 0), 1.15m, Priority: 1),
            new TariffPeriod(TariffType.Evening,  new TimeOnly(18, 0), new TimeOnly(23, 0), 0.80m)
        });


        public TariffSchedule(IReadOnlyList<TariffPeriod> periods)
        {
            if (periods.Count == 0)
            {
                throw new ArgumentException("Schedule must have at least one period.", nameof(periods));
            }

            _periods = periods;
        }


        // Розділяє бронювання на сегменти, щоб для кожного застосувати відповідний тариф.
        // Бронювання 17:00-19:00 дає два відрізки: годину за базовою ставкою і годину зі знижкою 20%.
        public IReadOnlyList<TariffSegment> Split(TimeRange booking)
        {
            List<DateTime> splitTimes = GetSplitTimes(booking);
            List<TariffSegment> segments = new(splitTimes.Count - 1);

            for (int i = 0; i < splitTimes.Count - 1; i++)
            {
                TimeRange range = new(splitTimes[i], splitTimes[i + 1]);
                TariffPeriod tariff = GetTariffAt(range.Start);

                segments.Add(new TariffSegment(range, tariff));
            }

            return segments;
        }


        // Моменти, у яких треба різати бронювання: його власні межі плюс межі тарифів усередині.
        // Для 17:00-19:00 це 17:00, 18:00 і 19:00, бо о 18:00 змінюється тариф.
        private List<DateTime> GetSplitTimes(TimeRange booking)
        {
            SortedSet<DateTime> splitTimes = new() { booking.Start, booking.End };

            // У TariffPeriod межі задані як TimeOnly, тобто час доби без дати.
            // Тому прикладаємо їх до кожної дати, якої торкається бронювання.

            for (DateTime date = booking.Start.Date; date <= booking.End.Date; date = date.AddDays(1))
            {
                foreach (var period in _periods)
                {
                    DateTime periodStart = date + period.Start.ToTimeSpan();
                    DateTime periodEnd = date + period.End.ToTimeSpan();

                    if (booking.Contains(periodStart))
                    {
                        splitTimes.Add(periodStart);
                    }

                    if (booking.Contains(periodEnd))
                    {
                        splitTimes.Add(periodEnd);
                    }
                }
            }

            return splitTimes.ToList();
        }


        // Якщо moment накривають кілька періодiв, перемагає той, у якого Priority більший.
        private TariffPeriod GetTariffAt(DateTime moment)
        {
            var timeOfDay = TimeOnly.FromDateTime(moment);

            TariffPeriod? tariff = _periods
                .Where(period => period.Covers(timeOfDay))
                .MaxBy(period => period.Priority);

            if (tariff is null)
            {
                throw new BusinessRuleException(
                    $"No tariff for {moment:HH:mm}. Booking is allowed only during working hours.");
            }

            return tariff;
        }


    }
}
