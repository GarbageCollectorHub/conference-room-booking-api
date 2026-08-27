
namespace RoomBooking.Domain.Exceptions
{
    public class ConflictException : DomainException
    {
        // Дія суперечить поточному стану даних: наприклад, зал на цей час уже зайнятий.
        // Шар API повертає 409.

        public ConflictException(string message) : base(message)
        {
        }
    }

}
