using FluentValidation;
using AuthService.Contracts;

namespace AuthService.Validators
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email é obrigatório.")
                .EmailAddress()
                .WithMessage("Email inválido.")
                .MaximumLength(255);

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password é obrigatória.")
                .MinimumLength(6)
                .WithMessage("Password deve ter no mínimo 6 caracteres.")
                .MaximumLength(100);
        }
    }
}