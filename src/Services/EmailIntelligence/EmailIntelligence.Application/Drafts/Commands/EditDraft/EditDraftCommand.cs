namespace EmailIntelligence.Application.Drafts.Commands.EditDraft;

public record EditDraftCommand(
    Guid DraftId,
    string UserId,
    string EditedContent,
    string[] EditTypes // tone, content, structure, length, etc.
) : ICommand<EditDraftResult>;

public record EditDraftResult(
    Guid DraftId,
    bool Success
);
