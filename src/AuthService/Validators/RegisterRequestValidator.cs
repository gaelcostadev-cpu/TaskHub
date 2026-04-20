using FluentValidation;
using AuthService.Contracts;

namespace AuthService.Validators
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email é obrigatório.")
                .EmailAddress()
                .WithMessage("Email inválido.")
                .MaximumLength(255);

            RuleFor(x => x.Username)
                .NotEmpty()
                .WithMessage("Nome de usuário é obrigatório.")
                .MinimumLength(3)
                .WithMessage("Nome de usuário deve ter no mínimo 3 caracteres.")
                .MaximumLength(30)
                .WithMessage("Nome de usuário deve ter no máximo 30 caracteres.")
                .Matches("^[a-zA-Z0-9_]+$")
                .WithMessage("Nome de usuário deve conter apenas letras, números ou underscore.");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Senha é obrigatória.")
                .MinimumLength(6)
                .WithMessage("Senha deve ter no mínimo 6 caracteres.")
                .MaximumLength(100)
                .WithMessage("Senha muito longa.")
                .Matches("[A-Z]")
                .WithMessage("Senha deve conter ao menos uma letra maiúscula.")
                .Matches("[a-z]")
                .WithMessage("Senha deve conter ao menos uma letra minúscula.")
                .Matches("[0-9]")
                .WithMessage("Senha deve conter ao menos um número.");
        }
    }
}