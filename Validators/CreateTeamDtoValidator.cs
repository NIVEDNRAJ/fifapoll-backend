using System;
using FluentValidation;
using FifaPollApi.DTOs.Team;

namespace FifaPollApi.Validators
{
    public class CreateTeamDtoValidator : AbstractValidator<CreateTeamDto>
    {
        public CreateTeamDtoValidator()
        {
            RuleFor(x => x.TeamName)
                .NotEmpty().WithMessage("Team name is required.")
                .MinimumLength(3).WithMessage("Team name must be at least 3 characters.")
                .MaximumLength(100).WithMessage("Team name cannot exceed 100 characters.");

            RuleFor(x => x.CountryCode)
                .NotEmpty().WithMessage("Country code is required.")
                .Length(3).WithMessage("Country code must be exactly 3 characters.");

            RuleFor(x => x.FlagUrl)
                .NotEmpty().WithMessage("Flag URL is required.")
                .Must(LinkMustBeAValidUri).WithMessage("Flag URL must be a valid HTTP/HTTPS URL.");
        }

        private bool LinkMustBeAValidUri(string link)
        {
            if (string.IsNullOrWhiteSpace(link)) return false;
            return Uri.TryCreate(link, UriKind.Absolute, out var outUri) 
                   && (outUri.Scheme == Uri.UriSchemeHttp || outUri.Scheme == Uri.UriSchemeHttps);
        }
    }
}
