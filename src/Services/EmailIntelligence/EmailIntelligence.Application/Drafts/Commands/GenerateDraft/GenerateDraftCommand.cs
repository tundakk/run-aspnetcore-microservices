namespace EmailIntelligence.Application.Drafts.Commands.GenerateDraft;

public record GenerateDraftCommand(
    Guid ProcessedEmailId,
    string UserId,
    string? AdditionalContext = null
) : ICommand<GenerateDraftResult>;

public record GenerateDraftResult(
    Guid DraftId,
    string GeneratedContent,
    double ConfidenceScore
);
