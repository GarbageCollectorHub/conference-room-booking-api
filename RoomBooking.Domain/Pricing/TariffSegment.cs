using RoomBooking.Domain.Shared;

namespace RoomBooking.Domain.Pricing
{

    // Частина бронювання з однією тарифною ставкою
    // Вартість бронювання = сума таких частин.

    public sealed record TariffSegment(TimeRange Range, TariffPeriod Tariff);

}
