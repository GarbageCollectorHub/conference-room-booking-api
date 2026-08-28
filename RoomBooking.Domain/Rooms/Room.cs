using RoomBooking.Domain.Exceptions;

namespace RoomBooking.Domain.Rooms
{
    public sealed class Room
    {
        public const int NameMaxLength = 100;

        private readonly List<Amenity> _amenities = new();
        public IReadOnlyList<Amenity> Amenities => _amenities;


        public Guid Id { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public int Capacity { get; private set; }
        public decimal PricePerHour { get; private set; }

        // Тарифи рахуються за місцевим часом залу а не в клієнта.
        // Бронювання при цьому зберігаються в UTC.
        public string TimeZoneId { get; private set; } = string.Empty;
        public bool IsDeleted { get; private set; }

        public Room(string name, int capacity, decimal pricePerHour, string timeZoneId)
        {
            Id = Guid.NewGuid();

            Rename(name);
            ChangeCapacity(capacity);
            ChangePricePerHour(pricePerHour);
            ChangeTimeZone(timeZoneId);
        }
        private Room()
        {
        }

        // Зал не видаляємо, оскільки на нього можуть посилатися минулі бронювання та звіти
        public void MarkDeleted()
        {
            IsDeleted = true;
        }

        public void Rename(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new BusinessRuleException("Room name is required.");
            }

            if(name.Length > NameMaxLength)
            {
                throw new BusinessRuleException($"Room name must be at most {NameMaxLength} characters.");
            }

            Name = name;
        }

        public void ChangeCapacity(int capacity)
        {
            if (capacity <= 0)
            {
                throw new BusinessRuleException("Room capacity must be greater than zero.");
            }

            Capacity = capacity;
        }

        public void ChangePricePerHour(decimal pricePerHour)
        {
            if (pricePerHour <= 0)
            {
                throw new BusinessRuleException("Room price per hour must be greater than zero.");
            }

            PricePerHour = pricePerHour;
        }

        public void ChangeTimeZone(string timeZoneId)
        {
            if (!TimeZoneInfo.TryFindSystemTimeZoneById(timeZoneId, out _))
            {
                throw new BusinessRuleException($"Unknown time zone '{timeZoneId}'.");
            }

            TimeZoneId = timeZoneId;
        }

        public DateTime ToLocalTime(DateTime utc)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(utc, GetTimeZone());
        }

        public DateTime ToUtc(DateTime localTime)
        {
            TimeZoneInfo zone = GetTimeZone();

            // Весной при переводе часов местного времени 03:00–04:00 в этот день не бывает
            // и ConvertTimeToUtc бросит ArgumentException — то есть 500 вместо внятного ответа
            if (zone.IsInvalidTime(localTime))
            {
                throw new BusinessRuleException($"Time {localTime:HH:mm} does not exist in {TimeZoneId} on this date.");
            }

            return TimeZoneInfo.ConvertTimeToUtc(localTime, zone);
        }


        public void AddAmenity(Amenity amenity)
        {
            if (_amenities.Any(item => item.Name == amenity.Name))
            {
                throw new ConflictException($"Room already has amenity '{amenity.Name}'.");
            }

            _amenities.Add(amenity);
        }

        public IReadOnlyList<Amenity> GetAmenities(IEnumerable<Guid> amenityIds)
        {
            List<Amenity> selected = new();

            foreach (Guid id in amenityIds)
            {
                Amenity? amenity = _amenities.FirstOrDefault(item => item.Id == id);

                if (amenity is null)
                {
                    throw new NotFoundException($"Room does not have amenity {id}.");
                }

                selected.Add(amenity);
            }

            return selected;
        }

        // Послуги оплачуються один раз за бронювання, тому просто сума цін.
        // Знижки і націнки на них не поширюються - вони діють лише на оренду залу.
        public decimal GetAmenitiesPrice(IEnumerable<Guid> amenityIds)
        {
            return GetAmenities(amenityIds).Sum(amenity => amenity.Price);
        }

        private TimeZoneInfo GetTimeZone()
        {
            return TimeZoneInfo.FindSystemTimeZoneById(TimeZoneId);
        }



    }
}
