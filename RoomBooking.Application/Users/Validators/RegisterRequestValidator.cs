using FluentValidation;
using RoomBooking.Application.Users.DTOs;
using RoomBooking.Domain.Users;

namespace RoomBooking.Application.Users.Validators
{
    public sealed class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(request => request.Email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .EmailAddress()
                .MaximumLength(User.EmailMaxLength);

            RuleFor(request => request.Password)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .MinimumLength(AuthService.PasswordMinLength);
        }
    }
}