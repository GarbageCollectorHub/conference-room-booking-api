using RoomBooking.Application.Users.DTOs;
using Swashbuckle.AspNetCore.Filters;

namespace RoomBooking.Api.SwaggerExamples
{

    public sealed class LoginRequestExample : IExamplesProvider<LoginRequest>
    {
        public LoginRequest GetExamples()
        {
            return new LoginRequest("admin@example.com", "admin");
        }
    }

    public sealed class RegisterRequestExample : IExamplesProvider<RegisterRequest>
    {
        public RegisterRequest GetExamples()
        {
            return new RegisterRequest("new.user@example.com", "password");
        }
    }

}