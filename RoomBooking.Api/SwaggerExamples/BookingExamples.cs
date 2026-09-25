using RoomBooking.Application.Bookings.DTOs;
using Swashbuckle.AspNetCore.Filters;

namespace RoomBooking.Api.SwaggerExamples
{

    public sealed class CreateBookingRequestExample : IExamplesProvider<CreateBookingRequest>
    {
        public CreateBookingRequest GetExamples()
        {
            return new CreateBookingRequest(
                Guid.Empty,
                new DateTimeOffset(2027, 7, 15, 10, 0, 0, TimeSpan.FromHours(3)),
                new DateTimeOffset(2027, 7, 15, 14, 0, 0, TimeSpan.FromHours(3)),
                []);
        }
    }

}