using DTOs;
using FluentValidation;

namespace Validators;

public class TrackDtoValidator : AbstractValidator<TrackDto>
{
    public TrackDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().MaximumLength(100);

        RuleFor(x => x.DurationSeconds)
            .GreaterThan(0).WithMessage("Track duration must be positive")
            .LessThan(3600).WithMessage("Track cannot be longer than 1 hour");

        RuleFor(x => x.Genre)
            .NotEmpty().MaximumLength(50);
    }
}
