
namespace RoomBooking.Domain.Exceptions
{
    // Порушено бізнес-правило: наприклад, час бронювання поза робочими годинами.
    // Шар API повертає 400.

    public class BusinessRuleException : DomainException
    {
        public BusinessRuleException(string message) : base(message)
        {
        }
    }
}
