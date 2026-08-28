using RoomBooking.Domain.Exceptions;
using RoomBooking.Domain.Pricing;
using RoomBooking.Domain.Rooms;
using RoomBooking.Domain.Shared;


namespace RoomBooking.Domain.Bookings
{
    public sealed class Booking
    {
        private readonly List<Amenity> _amenities = new();
        public IReadOnlyList<Amenity> Amenities => _amenities;

        public Guid Id { get; private set; }
        public Guid RoomId { get; private set; }

        // UTC. Місцевий час отримуємо через Room.ToLocalTime, коли рахуємо тарифи.
        public DateTime Start { get; private set; }
        public DateTime End { get; private set; }

        // Фіксована ціна на момент бронювання
        public decimal TotalPrice { get; private set; }

        public DateTime CreatedAt { get; private set; }
        
        public TimeRange Period => new(Start, End);

        public Booking(Room room, TimeRange localPeriod, IReadOnlyList<Amenity> amenities, BookingPrice price)
        {
            // Перевіряємо місцевий час, а не UTC: є зони зі зміщенням на пів години
            RequireWholeHours(localPeriod);

            Id = Guid.NewGuid();
            RoomId = room.Id;
            Start = room.ToUtc(localPeriod.Start);
            End = room.ToUtc(localPeriod.End);
            TotalPrice = price.Total;
            CreatedAt = DateTime.UtcNow;

            _amenities.AddRange(amenities);
        }

        private Booking() { }


        // Зал бронюють слотами, тому початок і кінець мають припадати на цілу годину.
        private static void RequireWholeHours(TimeRange period)
        {
            if (period.Start.Minute != 0 || period.Start.Second != 0
                || period.End.Minute != 0 || period.End.Second != 0)
            {
                throw new BusinessRuleException("Booking must start and end on a full hour.");
            }
        }


    }
}
