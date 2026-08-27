using RoomBooking.Domain.Shared;


namespace RoomBooking.Domain.Pricing
{

    // Бронювання, що зачіпає кілька тарифів, ділиться на частини: 17:00-19:00 дає дві.
    // RentalLine - одна така частина
    // Cost - сума за неї цілком, а не за годину.
    public sealed record RentalCharge(TariffType Tariff, TimeRange Range, decimal Cost);

}