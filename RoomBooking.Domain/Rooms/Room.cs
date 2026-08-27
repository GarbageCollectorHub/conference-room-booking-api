using RoomBooking.Domain.Exceptions;

namespace RoomBooking.Domain.Rooms
{
    public sealed class Room
    {
        public const int NameMaxLength = 100;

        private readonly List<Amenity> _amenities = new();
        public IReadOnlyList<Amenity> Amenities => _amenities;


        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public int Capacity { get; private set; }
        public decimal PricePerHour { get; private set; }

        public Room(string name, int capacity, decimal pricePerHour)
        {
            Id = Guid.NewGuid();

            Rename(name);
            ChangeCapacity(capacity);
            ChangePricePerHour(pricePerHour);
        }
        private Room()
        {
            Name = string.Empty;
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

        public void AddAmenity(Amenity amenity)
        {
            if (_amenities.Any(item => item.Name == amenity.Name))
            {
                throw new ConflictException($"Room already has amenity '{amenity.Name}'.");
            }

            _amenities.Add(amenity);
        }


        // Послуги оплачуються один раз за бронювання, тому просто сума цін.
        // Знижки і націнки на них не поширюються - вони діють лише на оренду залу.
        public decimal GetAmenitiesPrice(IEnumerable<Guid> amenityIds)
        {
            decimal total = 0;

            foreach (Guid id in amenityIds)
            {
                Amenity? amenity = _amenities.FirstOrDefault(item => item.Id == id);

                if (amenity is null)
                {
                    throw new NotFoundException($"Room does not have amenity {id}.");
                }

                total += amenity.Price;
            }

            return total;
        }


    }
}
