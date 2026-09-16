using FluentValidation;
using RoomBooking.Application.Rooms.DTOs;
using RoomBooking.Domain.Rooms;

namespace RoomBooking.Application.Rooms.Validators
{
    public sealed class CreateRoomRequestValidator : AbstractValidator<CreateRoomRequest>
    {
        public CreateRoomRequestValidator()
        {
            RuleFor(request => request.Name)
                .NotEmpty()
                .MaximumLength(Room.NameMaxLength);

            RuleFor(request => request.Capacity).GreaterThan(0);

            RuleFor(request => request.PricePerHour).GreaterThan(0);

            RuleFor(request => request.TimeZoneId).NotEmpty();

            // RuleForEach і SetValidator пропускають null, тому null-список і null елементи перевіряємо окремо.
            RuleFor(request => request.Amenities).NotNull();

            RuleForEach(request => request.Amenities)
                .NotNull()
                .SetValidator(new AmenityRequestValidator());
        }
    }
}