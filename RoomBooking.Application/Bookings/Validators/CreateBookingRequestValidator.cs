using FluentValidation;
using RoomBooking.Application.Bookings.DTOs;

namespace RoomBooking.Application.Bookings.Validators
{
    public sealed class CreateBookingRequestValidator : AbstractValidator<CreateBookingRequest>
    {
        public CreateBookingRequestValidator()
        {
            RuleFor(request => request.RoomId).NotEmpty();

            RuleFor(request => request.Start).NotEmpty();

            RuleFor(request => request.End)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .GreaterThan(request => request.Start)
                .WithMessage("End must be later than Start.");

            // RuleForEach пропускає null список, тому null перевіряємо окремо
            RuleFor(request => request.AmenityIds).NotNull();

            RuleForEach(request => request.AmenityIds).NotEmpty();
        }
    }
}