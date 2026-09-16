using FluentValidation;
using RoomBooking.Application.Reports.DTOs;

namespace RoomBooking.Application.Reports.Validators
{
    public sealed class ReportPeriodQueryValidator : AbstractValidator<ReportPeriodQuery>
    {
        public ReportPeriodQueryValidator()
        {
            RuleFor(query => query.From).NotEmpty();

            RuleFor(query => query.To)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .GreaterThan(query => query.From)
                .WithMessage("To must be later than From.");
        }
    }
}