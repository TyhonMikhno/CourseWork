using DTOs;
using FluentValidation;
using System.Linq;

namespace Validators;

public class PlaylistDtoValidator : AbstractValidator<PlaylistDto>
{
    public PlaylistDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().MaximumLength(100);

        RuleFor(x => x.Description)
            .MaximumLength(300);

        RuleForEach(x => x.TrackIds)
            .GreaterThan(0).WithMessage("TrackId must be positive");

        RuleFor(x => x.TrackIds)
            .Must(list => list == null || list.Distinct().Count() == list.Count)
            .WithMessage("TrackIds must be unique within the playlist");
    }
}
