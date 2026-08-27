
namespace RoomBooking.Domain.Pricing
{

    // Тарифний інтервал доби:

    // Multiplier - множник до базової ставки залу за годину:  0.90 це знижка 10%, 1.00 базова ставка, 1.15 націнка 15%
    // Priority   - пріоритет при пересечении перiодiв, більший має перевагу

    public sealed record TariffPeriod(
        TariffType Type,
        TimeOnly Start,
        TimeOnly End,
        decimal Multiplier,
        int Priority = 0
        )

    {
        public bool Covers(TimeOnly moment)
        {
            return moment >= Start && moment < End;
        }


    }
}
