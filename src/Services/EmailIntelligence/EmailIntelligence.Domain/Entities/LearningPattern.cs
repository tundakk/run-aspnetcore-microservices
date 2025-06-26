using EmailIntelligence.Domain.Enums;

namespace EmailIntelligence.Domain.Entities;

public class LearningPattern
{
    public Guid Id { get; private set; }
    public string UserId { get; private set; } = default!;
    public string PatternType { get; private set; } = default!; // "tone_adjustment", "category_correction", "priority_adjustment"
    public string OriginalContent { get; private set; } = default!;
    public string ModifiedContent { get; private set; } = default!;
    public float[] SemanticDifference { get; private set; } = default!;
    public Dictionary<string, object> Context { get; private set; } = new();
    public double ConfidenceScore { get; private set; }
    public int UsageCount { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime LastUsedAt { get; private set; }

    private LearningPattern() { } // EF Core constructor

    public static LearningPattern Create(
        string userId,
        string patternType,
        string originalContent,
        string modifiedContent,
        float[] semanticDifference,
        Dictionary<string, object>? context = null,
        double confidenceScore = 0.5)
    {
        return new LearningPattern
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            PatternType = patternType,
            OriginalContent = originalContent,
            ModifiedContent = modifiedContent,
            SemanticDifference = semanticDifference,
            Context = context ?? new Dictionary<string, object>(),
            ConfidenceScore = confidenceScore,
            UsageCount = 1,
            CreatedAt = DateTime.UtcNow,
            LastUsedAt = DateTime.UtcNow
        };
    }

    public void IncrementUsage()
    {
        UsageCount++;
        LastUsedAt = DateTime.UtcNow;
        
        // Increase confidence with usage
        ConfidenceScore = Math.Min(1.0, ConfidenceScore + 0.1);
    }

    public void UpdateConfidence(double newScore)
    {
        ConfidenceScore = Math.Max(0.0, Math.Min(1.0, newScore));
    }

    public bool IsApplicable(string content, Dictionary<string, object>? context = null)
    {
        // Simple applicability check - can be enhanced with more sophisticated logic
        var similarity = CalculateContentSimilarity(content, OriginalContent);
        var contextMatch = context == null || CheckContextSimilarity(context);
        
        return similarity > 0.7 && contextMatch && ConfidenceScore > 0.6;
    }

    private double CalculateContentSimilarity(string content1, string content2)
    {
        // Simple Jaccard similarity for now - could be enhanced with embeddings
        var words1 = content1.ToLowerInvariant().Split(' ', StringSplitOptions.RemoveEmptyEntries).ToHashSet();
        var words2 = content2.ToLowerInvariant().Split(' ', StringSplitOptions.RemoveEmptyEntries).ToHashSet();
        
        var intersection = words1.Intersect(words2).Count();
        var union = words1.Union(words2).Count();
        
        return union == 0 ? 0.0 : (double)intersection / union;
    }

    private bool CheckContextSimilarity(Dictionary<string, object> otherContext)
    {
        if (Context.Count == 0 && otherContext.Count == 0) return true;
        if (Context.Count == 0 || otherContext.Count == 0) return false;

        var matchingKeys = Context.Keys.Intersect(otherContext.Keys).Count();
        var totalKeys = Context.Keys.Union(otherContext.Keys).Count();
        
        return (double)matchingKeys / totalKeys > 0.5;
    }
}
