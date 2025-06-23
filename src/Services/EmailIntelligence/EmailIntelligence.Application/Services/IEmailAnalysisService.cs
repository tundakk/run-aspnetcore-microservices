namespace EmailIntelligence.Application.Services;

public interface IEmailAnalysisService
{
    Task<EmailAnalysisResult> AnalyzeEmailAsync(
        string subject, 
        string body, 
        string from, 
        string userId, 
        CancellationToken cancellationToken = default);
}

public record EmailAnalysisResult(
    EmailPriority Priority,
    EmailCategory Category,
    bool RequiresResponse,
    double ConfidenceScore,
    string[] Keywords,
    string? ActionItems
);
