using FluentValidation;
using AuthService.Contracts;

namespace AuthService.Validators
{
    public class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
    {
        public RefreshTokenRequestValidator()
        {
            RuleFor(x => x.RefreshToken)
                .NotEmpty()
                .WithMessage("RefreshToken é obrigatório.")
                .MinimumLength(32)
                .WithMessage("RefreshToken inválido.")
                .MaximumLength(500);
        }
    }
}