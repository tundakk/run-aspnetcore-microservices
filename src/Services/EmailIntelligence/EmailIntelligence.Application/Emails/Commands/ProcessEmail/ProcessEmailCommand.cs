namespace EmailIntelligence.Application.Emails.Commands.ProcessEmail;

public record ProcessEmailCommand(
    string EmailId,
    string UserId,
    string Subject,
    string From,
    string To,
    string Body,
    DateTime ReceivedAt
) : ICommand<ProcessEmailResult>;

public record ProcessEmailResult(
    Guid ProcessedEmailId,
    EmailPriority Priority,
    EmailCategory Category,
    bool RequiresResponse,
    double ConfidenceScore,
    string[] Keywords,
    string? ActionItems
);
