namespace EmailIntelligence.Application.Services;

public interface IDraftGenerationService
{
    Task<DraftGenerationResult> GenerateDraftAsync(
        string originalSubject,
        string originalBody,
        string from,
        UserToneProfile? toneProfile,
        string? additionalContext = null,
        CancellationToken cancellationToken = default);
}

public record DraftGenerationResult(
    string Content,
    string ModelUsed,
    string Prompt,
    double Confidence,
    double QualityScore
);
