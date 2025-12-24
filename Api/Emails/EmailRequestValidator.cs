using FluentValidation;

namespace ApiEmail.Api.Emails;

public sealed class EmailRequestValidator : AbstractValidator<EmailRequest>
{
    public EmailRequestValidator()
    {
        RuleFor(x => x.SenderFirstName)
            .NotEmpty().WithMessage($"Parametr nemůže být prázdný.");

        RuleFor(x => x.SenderLastName)
            .NotEmpty().WithMessage($"Parametr nemůže být prázdný.");

        RuleFor(x => x.MessageBody)
            .NotEmpty().WithMessage($"Parametr nemůže být prázdný.");

        RuleFor(x => x.SenderAddress)
            .NotEmpty().WithMessage($"Parametr nemůže být prázdný.")
            .EmailAddress().WithMessage($"Parametr nemá platný formát e-mailové adresy.");

        RuleFor(x => x.Subject)
            .MaximumLength(200).WithMessage($"Parametr může mít maximálně 200 znaků.");
    }
}