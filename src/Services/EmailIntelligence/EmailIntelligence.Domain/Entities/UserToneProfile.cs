namespace EmailIntelligence.Domain.Entities;

public class UserToneProfile
{
    public Guid Id { get; private set; }
    public string UserId { get; private set; } = default!;
    public string ToneCharacteristics { get; private set; } = default!; // JSON stored tone analysis
    public string PreferredPhrases { get; private set; } = default!; // JSON array of phrases
    public string AvoidedPhrases { get; private set; } = default!; // JSON array of phrases to avoid
    public string WritingStyle { get; private set; } = default!; // formal, casual, friendly, etc.
    public double ConfidenceLevel { get; private set; } // How confident we are in this profile
    public DateTime CreatedAt { get; private set; }
    public DateTime LastUpdated { get; private set; }
    public int EmailsSampled { get; private set; } // Number of emails used to build this profile
    
    private UserToneProfile() { } // EF Core constructor
    
    public static UserToneProfile Create(string userId, string toneCharacteristics, string writingStyle)
    {
        return new UserToneProfile
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ToneCharacteristics = toneCharacteristics,
            PreferredPhrases = "[]",
            AvoidedPhrases = "[]",
            WritingStyle = writingStyle,
            ConfidenceLevel = 0.1, // Start with low confidence
            CreatedAt = DateTime.UtcNow,
            LastUpdated = DateTime.UtcNow,
            EmailsSampled = 0
        };
    }
    
    public void UpdateProfile(string toneCharacteristics, string preferredPhrases, string avoidedPhrases, 
        string writingStyle, int additionalEmailsSampled)
    {
        ToneCharacteristics = toneCharacteristics;
        PreferredPhrases = preferredPhrases;
        AvoidedPhrases = avoidedPhrases;
        WritingStyle = writingStyle;
        EmailsSampled += additionalEmailsSampled;
        
        // Increase confidence as we sample more emails, but cap at 0.95
        ConfidenceLevel = Math.Min(0.95, ConfidenceLevel + (additionalEmailsSampled * 0.05));
        LastUpdated = DateTime.UtcNow;
    }
}
