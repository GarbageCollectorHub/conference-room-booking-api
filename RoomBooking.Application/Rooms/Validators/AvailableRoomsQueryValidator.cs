using FluentValidation;
using RoomBooking.Application.Rooms.DTOs;

namespace RoomBooking.Application.Rooms.Validators
{
    public sealed class AvailableRoomsQueryValidator : AbstractValidator<AvailableRoomsQuery>
    {
        public AvailableRoomsQueryValidator()
        {
            RuleFor(query => query.Start).NotEmpty();

            RuleFor(query => query.End)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .GreaterThan(query => query.Start)
                .WithMessage("End must be later than Start.");

            // 0 означає без обмеження за місткістю
            RuleFor(query => query.Capacity).GreaterThanOrEqualTo(0);
        }
    }
}