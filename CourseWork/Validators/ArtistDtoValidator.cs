using DTOs;
using FluentValidation;

namespace Validators;

public class ArtistDtoValidator : AbstractValidator<ArtistDto>
{
    public ArtistDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Artist name is required")
            .MinimumLength(2)
            .MaximumLength(100);

        RuleFor(x => x.Age)
            .InclusiveBetween(10, 120).WithMessage("Age must be between 10 and 120");

        RuleFor(x => x.Country)
            .NotEmpty().WithMessage("Country is required")
            .MaximumLength(50);

        RuleFor(x => x.Label)
            .MaximumLength(50);
    }
}
