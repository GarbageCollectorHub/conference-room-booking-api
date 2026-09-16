using FluentValidation;
using RoomBooking.Application.Users.DTOs;

namespace RoomBooking.Application.Users.Validators
{
    public sealed class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(request => request.Email).NotEmpty();

            RuleFor(request => request.Password).NotEmpty();
        }
    }
}