using EmailIntelligence.Application.Services;
using EmailIntelligence.Domain.Repositories;
using BuildingBlocks.CQRS;

namespace EmailIntelligence.Application.Drafts.Commands.EditDraft;

public class EditDraftHandler(
    IEmailDraftRepository draftRepository,
    ILearningService learningService)
    : ICommandHandler<EditDraftCommand, EditDraftResult>
{
    public async Task<EditDraftResult> Handle(EditDraftCommand command, CancellationToken cancellationToken)
    {
        var draft = await draftRepository.GetByIdAsync(command.DraftId, cancellationToken);
        if (draft == null)
            throw new ArgumentException($"Draft with ID {command.DraftId} not found");

        // Store original content for learning
        var originalContent = draft.GeneratedContent;
        
        // Update draft with user edits
        draft.EditByUser(command.EditedContent, command.EditTypes);
        await draftRepository.UpdateAsync(draft, cancellationToken);

        // Learn from user edits for continuous improvement
        await learningService.LearnFromDraftEditAsync(
            command.UserId,
            originalContent,
            command.EditedContent,
            command.EditTypes,
            cancellationToken);

        // TODO: Publish event for other services to learn when MassTransit is properly configured
        
        return new EditDraftResult(command.DraftId, true);
    }
}
