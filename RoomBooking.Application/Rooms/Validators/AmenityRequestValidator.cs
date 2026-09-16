using FluentValidation;
using RoomBooking.Application.Rooms.DTOs;
using RoomBooking.Domain.Rooms;

namespace RoomBooking.Application.Rooms.Validators
{
    public sealed class AmenityRequestValidator : AbstractValidator<AmenityRequest>
    {
        public AmenityRequestValidator()
        {
            RuleFor(request => request.Name)
                .NotEmpty()
                .MaximumLength(Amenity.NameMaxLength);

            RuleFor(request => request.Price).GreaterThanOrEqualTo(0);
        }
    }
}