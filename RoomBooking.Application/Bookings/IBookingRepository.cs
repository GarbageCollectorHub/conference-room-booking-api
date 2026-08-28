using RoomBooking.Domain.Bookings;
using RoomBooking.Domain.Shared;

namespace RoomBooking.Application.Bookings
{
    public interface IBookingRepository
    {

        /// <summary>Транзакція: перевірка вільного часу плюс вставка.</summary>
        /// <returns><c>false</c>, якщо зал зайнятий.</returns>
        Task<bool> TryAddAsync(Booking booking, CancellationToken cancellationToken);

        Task<bool> HasBookingAsync(Guid roomId, TimeRange utcPeriod, CancellationToken cancellationToken);

        Task<IReadOnlyList<Booking>> GetInPeriodAsync(TimeRange utcPeriod, CancellationToken cancellationToken);

        Task<bool> HasFutureBookingsAsync(Guid roomId, CancellationToken cancellationToken);

    }
}
