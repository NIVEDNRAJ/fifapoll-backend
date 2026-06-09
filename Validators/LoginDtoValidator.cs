using FluentValidation;
using FifaPollApi.DTOs.Auth;

namespace FifaPollApi.Validators
{
    public class LoginDtoValidator : AbstractValidator<LoginDto>
    {
        public Task<bool> IsValidAsync(LoginDto model) => Task.FromResult(true); // just a placeholder or helper if needed, but FluentValidation uses ValidationResult

        public LoginDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("A valid email address is required.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.");
        }
    }
}
