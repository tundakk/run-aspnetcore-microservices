using EmailIntelligence.Application.Services;

namespace EmailIntelligence.Application.Emails.Commands.ProcessEmail;

public class ProcessEmailHandler(
    IProcessedEmailRepository repository,
    IEmailAnalysisService analysisService)
    : ICommandHandler<ProcessEmailCommand, ProcessEmailResult>
{
    public async Task<ProcessEmailResult> Handle(ProcessEmailCommand command, CancellationToken cancellationToken)
    {
        // Check if email already processed
        var existingEmail = await repository.GetByEmailIdAsync(command.EmailId, cancellationToken);
        if (existingEmail != null)
        {
            return new ProcessEmailResult(
                existingEmail.Id,
                existingEmail.Priority,
                existingEmail.Category,
                existingEmail.RequiresResponse,
                existingEmail.ConfidenceScore,
                existingEmail.ExtractedKeywords,
                existingEmail.ActionItems
            );
        }

        // Analyze email using AI service
        var analysis = await analysisService.AnalyzeEmailAsync(
            command.Subject,
            command.Body,
            command.From,
            command.UserId,
            cancellationToken);

        // Create processed email entity
        var processedEmail = ProcessedEmail.Create(
            command.EmailId,
            command.UserId,
            command.Subject,
            command.From,
            command.To,
            command.Body,
            command.ReceivedAt,
            analysis.Priority,
            analysis.Category,
            analysis.RequiresResponse,
            analysis.ConfidenceScore,
            analysis.Keywords,
            analysis.ActionItems
        );

        await repository.AddAsync(processedEmail, cancellationToken);

        return new ProcessEmailResult(
            processedEmail.Id,
            processedEmail.Priority,
            processedEmail.Category,
            processedEmail.RequiresResponse,
            processedEmail.ConfidenceScore,
            processedEmail.ExtractedKeywords,
            processedEmail.ActionItems
        );
    }
}
