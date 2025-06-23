using EmailIntelligence.Application.Services;
using BuildingBlocks.Messaging.Events;
using MassTransit;

namespace EmailIntelligence.Application.Drafts.Commands.EditDraft;

public class EditDraftHandler(
    IEmailDraftRepository draftRepository,
    ILearningService learningService,
    IPublishEndpoint publisher)
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

        // Publish event for other services to learn
        var learningEvent = new DraftEditedEvent(
            command.UserId,
            command.DraftId,
            originalContent,
            command.EditedContent,
            command.EditTypes,
            DateTime.UtcNow
        );
        
        await publisher.Publish(learningEvent, cancellationToken);

        return new EditDraftResult(command.DraftId, true);
    }
}
