using EmailIntelligence.Application.Services;

namespace EmailIntelligence.Application.Drafts.Commands.GenerateDraft;

public class GenerateDraftHandler(
    IProcessedEmailRepository emailRepository,
    IEmailDraftRepository draftRepository,
    IUserToneProfileRepository toneRepository,
    IDraftGenerationService draftService)
    : ICommandHandler<GenerateDraftCommand, GenerateDraftResult>
{
    public async Task<GenerateDraftResult> Handle(GenerateDraftCommand command, CancellationToken cancellationToken)
    {
        // Get the processed email
        var processedEmail = await emailRepository.GetByIdAsync(command.ProcessedEmailId, cancellationToken);
        if (processedEmail == null)
            throw new ArgumentException($"Processed email with ID {command.ProcessedEmailId} not found");

        // Check if draft already exists
        var existingDraft = await draftRepository.GetByProcessedEmailIdAsync(command.ProcessedEmailId, cancellationToken);
        if (existingDraft != null)
        {
            return new GenerateDraftResult(
                existingDraft.Id,
                existingDraft.GeneratedContent,
                existingDraft.GenerationConfidence
            );
        }

        // Get user tone profile
        var toneProfile = await toneRepository.GetByUserIdAsync(command.UserId, cancellationToken);

        // Generate draft using AI service
        var draftResult = await draftService.GenerateDraftAsync(
            processedEmail.Subject,
            processedEmail.Body,
            processedEmail.From,
            toneProfile,
            command.AdditionalContext,
            cancellationToken);

        // Create draft entity
        var draft = EmailDraft.Create(
            command.ProcessedEmailId,
            command.UserId,
            processedEmail.Body,
            draftResult.Content,
            draftResult.ModelUsed,
            draftResult.Prompt,
            draftResult.Confidence,
            draftResult.QualityScore
        );

        await draftRepository.AddAsync(draft, cancellationToken);

        return new GenerateDraftResult(
            draft.Id,
            draft.GeneratedContent,
            draft.GenerationConfidence
        );
    }
}
