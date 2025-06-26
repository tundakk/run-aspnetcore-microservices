using Microsoft.SemanticKernel;

namespace EmailIntelligence.Application.Services;

public interface ISemanticKernelService
{
    Kernel GetKernel();
    Task<string> GenerateEmailDraftAsync(string subject, string description, string? userTone = null, string? contextualHistory = null, CancellationToken cancellationToken = default);
    Task<T> ExecuteSemanticFunctionAsync<T>(string functionName, object parameters, CancellationToken cancellationToken = default);
    Task<string> ClassifyEmailAsync(string subject, string body, string from, string? contextualHistory = null, CancellationToken cancellationToken = default);
    Task<string> AnalyzeToneAsync(string content, CancellationToken cancellationToken = default);
    Task<string> ExtractKeywordsAsync(string content, CancellationToken cancellationToken = default);
    Task<string> SummarizeContentAsync(string content, int maxWords = 100, CancellationToken cancellationToken = default);
}
