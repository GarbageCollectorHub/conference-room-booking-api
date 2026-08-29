using RoomBooking.Application.Rooms.DTOs;
using Swashbuckle.AspNetCore.Filters;

namespace RoomBooking.Api.SwaggerExamples
{

    public sealed class CreateRoomRequestExample : IExamplesProvider<CreateRoomRequest>
    {

        public CreateRoomRequest GetExamples()
        {
            return new CreateRoomRequest(
                "Test Hall",
                77,
                1777m,
                "Europe/Kyiv",
                [new AmenityRequest("Projector", 333m)]);
        }

        public sealed class UpdateRoomRequestExample : IExamplesProvider<UpdateRoomRequest>
        {
            public UpdateRoomRequest GetExamples()
            {
                return new UpdateRoomRequest("Hall A", 60, 2200m, "Europe/Kyiv");
            }
        }

    }
}