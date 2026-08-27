using RoomBooking.Domain.Exceptions;

namespace RoomBooking.Domain.Rooms
{


    // Послуга залу: проєктор, Wi-Fi, звук.
    public sealed class Amenity
    {
        public const int NameMaxLength = 100;

        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public decimal Price { get; private set; }

        public Amenity(string name, decimal price)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new BusinessRuleException("Amenity name is required.");
            }

            if (name.Length > NameMaxLength)
            {
                throw new BusinessRuleException($"Amenity name must be at most {NameMaxLength} characters.");
            }

            if (price < 0)
            {
                throw new BusinessRuleException("Amenity price cannot be negative.");
            }

            Id = Guid.NewGuid();
            Name = name;
            Price = price;
        }


        private Amenity()
        {
            Name = string.Empty;
        }

    }

}
