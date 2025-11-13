namespace BrainBay.API.Validators
{
    using BrainBay.Core.Entities;
    using BrainBay.Model.Inputs;
    using BrainBay.Model.Responses;
    using FluentValidation;

    public class CharacterValidator : AbstractValidator<CreateCharacterInput>
    {
        public CharacterValidator()
        {
            RuleFor(c => c.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

            RuleFor(c => c.Status)
                .NotEmpty().WithMessage("Status is required.")
                .MaximumLength(50)
                .Must(s => new[] { "Alive", "Dead", "unknown" }.Contains(s))
                .WithMessage("Status must be 'Alive', 'Dead', or 'unknown'.");

            RuleFor(c => c.Species)
                .MaximumLength(50).When(c => !string.IsNullOrEmpty(c.Species));

            RuleFor(c => c.Type)
                .MaximumLength(50).When(c => !string.IsNullOrEmpty(c.Type));

            RuleFor(c => c.Gender)
                .MaximumLength(20)
                .Must(g => new[] { "Male", "Female", "Genderless", "unknown" }.Contains(g))
                .When(c => !string.IsNullOrEmpty(c.Gender))
                .WithMessage("Gender must be one of: Male, Female, Genderless, unknown.");

            RuleFor(c => c.Image)
                .MaximumLength(300)
                .Matches(@"^https?://").When(c => !string.IsNullOrEmpty(c.Image))
                .WithMessage("Image must be a valid URL starting with http or https.");

            RuleFor(c => c.Episode)
                .NotNull().WithMessage("Episodes list cannot be null.")
                .Must(e => e.All(ep => Uri.IsWellFormedUriString(ep, UriKind.Absolute)))
                .When(c => c.Episode?.Any() == true)
                .WithMessage("All episodes must be valid URLs.");


            RuleFor(c => c.Location)
                .SetValidator(new LocationValidator())
                .When(c => c.Location != null);

            RuleFor(c => c.Origin)
                .SetValidator(new OriginValidator())
                .When(c => c.Origin != null);
        }
    }
    public class LocationValidator : AbstractValidator<LocationResponse?>
    {
        public LocationValidator()
        {
            RuleFor(l => l.Name)
                .NotEmpty().WithMessage("Location name is required.")
                .MaximumLength(100);

            RuleFor(l => l.Url)
                .MaximumLength(200)
                .Must(BeAValidUrl).When(l => !string.IsNullOrEmpty(l.Url))
                .WithMessage("Location URL must be valid (http/https).");
        }

        private bool BeAValidUrl(string? url)
            => Uri.TryCreate(url, UriKind.Absolute, out var result)
               && (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
    }
    public class OriginValidator : AbstractValidator<OriginResponse?>
    {
        public OriginValidator()
        {
            RuleFor(o => o.Name)
                .NotEmpty().WithMessage("Origin name is required.")
                .MaximumLength(100);

            RuleFor(o => o.Url)
                .MaximumLength(200)
                .Must(BeAValidUrl).When(o => !string.IsNullOrEmpty(o.Url))
                .WithMessage("Origin URL must be valid (http/https).");
        }

        private bool BeAValidUrl(string? url)
            => Uri.TryCreate(url, UriKind.Absolute, out var result)
               && (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
    }
}
