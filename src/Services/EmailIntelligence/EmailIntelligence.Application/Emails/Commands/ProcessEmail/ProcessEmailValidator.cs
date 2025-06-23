namespace EmailIntelligence.Application.Emails.Commands.ProcessEmail;

public class ProcessEmailValidator : AbstractValidator<ProcessEmailCommand>
{
    public ProcessEmailValidator()
    {
        RuleFor(x => x.EmailId)
            .NotEmpty()
            .WithMessage("Email ID is required");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required");

        RuleFor(x => x.Subject)
            .NotEmpty()
            .WithMessage("Subject is required");

        RuleFor(x => x.Body)
            .NotEmpty()
            .WithMessage("Email body is required");

        RuleFor(x => x.From)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("Valid sender email is required");

        RuleFor(x => x.To)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("Valid recipient email is required");
    }
}
