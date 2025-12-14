using DTOs;
using FluentValidation;

namespace Validators;

public class AlbumDtoValidator : AbstractValidator<AlbumDto>
{
    public AlbumDtoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().MaximumLength(100);

        RuleFor(x => x.Length)
            .GreaterThan(0).WithMessage("Album length must be positive");

        RuleFor(x => x.Year)
            .InclusiveBetween(1900, DateTime.Now.Year);

        RuleFor(x => x.SongCount)
            .GreaterThan(0).WithMessage("Album must have at least 1 song");

        RuleFor(x => x.Genre)
            .NotEmpty().MaximumLength(50);
    }
}
