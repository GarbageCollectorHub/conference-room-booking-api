
namespace RoomBooking.Domain.Pricing
{

    // Тарифний інтервал доби:

    // Multiplier - множник до базової ставки залу за годину:  0.90 це знижка 10%, 1.00 базова ставка, 1.15 націнка 15%
    // Priority   - пріоритет при пересечении перiодiв, більший має перевагу

    public sealed record TariffPeriod
    {
        public TariffType Type { get; }
        public TimeOnly Start { get; }
        public TimeOnly End { get; }
        public decimal Multiplier { get; }
        public int Priority { get; }

        public TariffPeriod(
            TariffType type,
            TimeOnly start,
            TimeOnly end,
            decimal multiplier,
            int priority = 0)
        {
            if (end <= start)
            {
                throw new ArgumentException("Period end must be later than its start.", nameof(end));
            }

            if (multiplier <= 0)
            {
                throw new ArgumentException("Multiplier must be greater than zero.", nameof(multiplier));
            }

            Type = type;
            Start = start;
            End = end;
            Multiplier = multiplier;
            Priority = priority;
        }

        public bool Covers(TimeOnly moment)
        {
            return moment >= Start && moment < End;
        }
    }


}
