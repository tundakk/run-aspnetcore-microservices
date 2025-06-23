namespace EmailIntelligence.Application.Services;

public interface ILearningService
{
    Task LearnFromDraftEditAsync(
        string userId,
        string originalContent,
        string editedContent,
        string[] editTypes,
        CancellationToken cancellationToken = default);
        
    Task LearnFromUserCorrectionsAsync(
        string userId,
        CancellationToken cancellationToken = default);
        
    Task UpdateUserToneProfileAsync(
        string userId,
        CancellationToken cancellationToken = default);
}
