using EmailIntelligence.Domain.Enums;

namespace EmailIntelligence.Domain.Entities;

public class EmailDraft
{
    public Guid Id { get; private set; }
    public Guid ProcessedEmailId { get; private set; }
    public string UserId { get; private set; } = default!;
    public string OriginalContent { get; private set; } = default!;
    public string GeneratedContent { get; private set; } = default!;
    public string? UserEditedContent { get; private set; }
    public string? FinalContent { get; private set; }
    public DraftStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? EditedAt { get; private set; }
    public DateTime? SentAt { get; private set; }
    
    // Learning metrics
    public double OriginalQualityScore { get; private set; }
    public int UserEditCount { get; private set; }
    public string[]? UserEditTypes { get; private set; } // tone, content, structure, etc.
    public bool WasApproved { get; private set; }
    
    // AI generation metadata
    public string ModelUsed { get; private set; } = default!;
    public string Prompt { get; private set; } = default!;
    public double GenerationConfidence { get; private set; }
    
    public ProcessedEmail ProcessedEmail { get; private set; } = default!;
    
    private EmailDraft() { } // EF Core constructor
    
    public static EmailDraft Create(
        Guid processedEmailId,
        string userId,
        string originalContent,
        string generatedContent,
        string modelUsed,
        string prompt,
        double generationConfidence,
        double qualityScore = 0.0)
    {
        return new EmailDraft
        {
            Id = Guid.NewGuid(),
            ProcessedEmailId = processedEmailId,
            UserId = userId,
            OriginalContent = originalContent,
            GeneratedContent = generatedContent,
            Status = DraftStatus.Generated,
            CreatedAt = DateTime.UtcNow,
            ModelUsed = modelUsed,
            Prompt = prompt,
            GenerationConfidence = generationConfidence,
            OriginalQualityScore = qualityScore
        };
    }
    
    public void EditByUser(string editedContent, string[] editTypes)
    {
        UserEditedContent = editedContent;
        UserEditTypes = editTypes;
        EditedAt = DateTime.UtcNow;
        UserEditCount++;
        Status = DraftStatus.UserEdited;
    }
    
    public void Approve(string? finalContent = null)
    {
        FinalContent = finalContent ?? UserEditedContent ?? GeneratedContent;
        Status = DraftStatus.Approved;
        WasApproved = true;
    }
    
    public void Reject()
    {
        Status = DraftStatus.Rejected;
        WasApproved = false;
    }
    
    public void MarkAsSent()
    {
        Status = DraftStatus.Sent;
        SentAt = DateTime.UtcNow;
    }
}
