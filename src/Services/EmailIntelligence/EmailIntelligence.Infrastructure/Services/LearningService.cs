using EmailIntelligence.Application.Services;
using EmailIntelligence.Domain.Entities;
using EmailIntelligence.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace EmailIntelligence.Infrastructure.Services;

public class LearningService(
    IEmailDraftRepository draftRepository,
    IProcessedEmailRepository emailRepository,
    IUserToneProfileRepository toneRepository,
    ILogger<LearningService> logger) : ILearningService
{
    public async Task LearnFromDraftEditAsync(
        string userId,
        string originalContent,
        string editedContent,
        string[] editTypes,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Analyze the differences between original and edited content
            var editAnalysis = AnalyzeEdits(originalContent, editedContent, editTypes);
            
            // Update user tone profile based on edits
            await UpdateUserToneFromEditsAsync(userId, editAnalysis, cancellationToken);
            
            logger.LogInformation("Learned from draft edit for user {UserId}: {EditTypes}", 
                userId, string.Join(", ", editTypes));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error learning from draft edit for user {UserId}", userId);
        }
    }

    public async Task LearnFromUserCorrectionsAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var corrections = await emailRepository.GetUserCorrectionsForLearningAsync(userId, cancellationToken);
            
            // Analyze patterns in user corrections
            foreach (var email in corrections)
            {
                if (email.UserCorrectedPriority && email.UserCorrectedPriorityValue.HasValue)
                {
                    // Learn from priority corrections
                    logger.LogInformation("User {UserId} corrected priority from {Original} to {Corrected}",
                        userId, email.Priority, email.UserCorrectedPriorityValue.Value);
                }
                
                if (email.UserCorrectedCategory && email.UserCorrectedCategoryValue.HasValue)
                {
                    // Learn from category corrections
                    logger.LogInformation("User {UserId} corrected category from {Original} to {Corrected}",
                        userId, email.Category, email.UserCorrectedCategoryValue.Value);
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error learning from user corrections for user {UserId}", userId);
        }
    }

    public async Task UpdateUserToneProfileAsync(
        string userId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var userDrafts = await draftRepository.GetUserEditedDraftsForLearningAsync(userId, cancellationToken);
            var profile = await toneRepository.GetByUserIdAsync(userId, cancellationToken);
            
            if (!userDrafts.Any())
                return;

            var toneAnalysis = AnalyzeToneFromDrafts(userDrafts);
            
            if (profile == null)
            {
                profile = UserToneProfile.Create(userId, toneAnalysis.ToneCharacteristics, toneAnalysis.WritingStyle);
                await toneRepository.AddAsync(profile, cancellationToken);
            }
            else
            {
                profile.UpdateProfile(
                    toneAnalysis.ToneCharacteristics,
                    toneAnalysis.PreferredPhrases,
                    toneAnalysis.AvoidedPhrases,
                    toneAnalysis.WritingStyle,
                    userDrafts.Count()
                );
                await toneRepository.UpdateAsync(profile, cancellationToken);
            }
            
            logger.LogInformation("Updated tone profile for user {UserId} based on {DraftCount} drafts",
                userId, userDrafts.Count());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating tone profile for user {UserId}", userId);
        }
    }

    private EditAnalysis AnalyzeEdits(string original, string edited, string[] editTypes)
    {
        // Simple analysis - in production, this would be more sophisticated
        var lengthDifference = edited.Length - original.Length;
        var isFormal = edited.Contains("Dear") || edited.Contains("Sincerely");
        var isCasual = edited.Contains("Hi") || edited.Contains("Thanks");
        
        return new EditAnalysis(
            editTypes,
            lengthDifference,
            isFormal,
            isCasual
        );
    }

    private async Task UpdateUserToneFromEditsAsync(
        string userId, 
        EditAnalysis analysis, 
        CancellationToken cancellationToken)
    {
        // Update tone profile based on edit patterns
        // This is a simplified implementation
        await Task.CompletedTask;
    }

    private ToneAnalysis AnalyzeToneFromDrafts(IEnumerable<EmailDraft> drafts)
    {
        // Analyze user's editing patterns to determine tone preferences
        var editedContents = drafts
            .Where(d => !string.IsNullOrEmpty(d.UserEditedContent))
            .Select(d => d.UserEditedContent!)
            .ToList();

        var isFormalStyle = editedContents.Any(c => 
            c.Contains("Dear") || c.Contains("Sincerely") || c.Contains("Best regards"));
            
        var isCasualStyle = editedContents.Any(c => 
            c.Contains("Hi") || c.Contains("Thanks") || c.Contains("Cheers"));

        var writingStyle = isFormalStyle ? "formal" : isCasualStyle ? "casual" : "professional";
        
        return new ToneAnalysis(
            $"Style: {writingStyle}, Analyzed from {editedContents.Count} samples",
            "[]", // Would extract actual preferred phrases
            "[]", // Would extract avoided phrases  
            writingStyle
        );
    }

    private record EditAnalysis(
        string[] EditTypes,
        int LengthDifference,
        bool IsFormal,
        bool IsCasual
    );

    private record ToneAnalysis(
        string ToneCharacteristics,
        string PreferredPhrases,
        string AvoidedPhrases,
        string WritingStyle
    );
}
