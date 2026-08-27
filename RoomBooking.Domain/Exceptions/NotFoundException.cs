
namespace RoomBooking.Domain.Exceptions
{
    public class NotFoundException : DomainException
    {
        // Запитаного обʼєкта не існує. Шар API повертає 404.

        public NotFoundException(string message) : base(message)
        {
        }
    }

}
