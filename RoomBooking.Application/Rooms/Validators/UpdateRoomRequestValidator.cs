using FluentValidation;
using RoomBooking.Application.Rooms.DTOs;
using RoomBooking.Domain.Rooms;

namespace RoomBooking.Application.Rooms.Validators
{
    public sealed class UpdateRoomRequestValidator : AbstractValidator<UpdateRoomRequest>
    {
        public UpdateRoomRequestValidator()
        {
            RuleFor(request => request.Name)
                .NotEmpty()
                .MaximumLength(Room.NameMaxLength);

            RuleFor(request => request.Capacity).GreaterThan(0);

            RuleFor(request => request.PricePerHour).GreaterThan(0);

            RuleFor(request => request.TimeZoneId).NotEmpty();
        }
    }
}