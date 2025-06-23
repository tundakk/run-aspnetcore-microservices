using EmailIntelligence.Domain.Enums;

namespace EmailIntelligence.Domain.Entities;

public class ProcessedEmail
{
    public Guid Id { get; private set; }
    public string EmailId { get; private set; } = default!;
    public string UserId { get; private set; } = default!;
    public string Subject { get; private set; } = default!;
    public string From { get; private set; } = default!;
    public string To { get; private set; } = default!;
    public string Body { get; private set; } = default!;
    public DateTime ReceivedAt { get; private set; }
    public DateTime ProcessedAt { get; private set; }
    
    // AI Classification Results
    public EmailPriority Priority { get; private set; }
    public EmailCategory Category { get; private set; }
    public bool RequiresResponse { get; private set; }
    public double ConfidenceScore { get; private set; }
    public string[] ExtractedKeywords { get; private set; } = Array.Empty<string>();
    public string? ActionItems { get; private set; }
    
    // Learning Data
    public bool UserCorrectedPriority { get; private set; }
    public EmailPriority? UserCorrectedPriorityValue { get; private set; }
    public bool UserCorrectedCategory { get; private set; }
    public EmailCategory? UserCorrectedCategoryValue { get; private set; }
    
    private ProcessedEmail() { } // EF Core constructor
    
    public static ProcessedEmail Create(
        string emailId,
        string userId,
        string subject,
        string from,
        string to,
        string body,
        DateTime receivedAt,
        EmailPriority priority,
        EmailCategory category,
        bool requiresResponse,
        double confidenceScore,
        string[] keywords,
        string? actionItems = null)
    {
        return new ProcessedEmail
        {
            Id = Guid.NewGuid(),
            EmailId = emailId,
            UserId = userId,
            Subject = subject,
            From = from,
            To = to,
            Body = body,
            ReceivedAt = receivedAt,
            ProcessedAt = DateTime.UtcNow,
            Priority = priority,
            Category = category,
            RequiresResponse = requiresResponse,
            ConfidenceScore = confidenceScore,
            ExtractedKeywords = keywords,
            ActionItems = actionItems
        };
    }
    
    public void CorrectPriority(EmailPriority correctedPriority)
    {
        UserCorrectedPriority = true;
        UserCorrectedPriorityValue = correctedPriority;
    }
    
    public void CorrectCategory(EmailCategory correctedCategory)
    {
        UserCorrectedCategory = true;
        UserCorrectedCategoryValue = correctedCategory;
    }
}
