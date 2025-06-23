namespace EmailIntelligence.Infrastructure.Configuration;

public class LLMSettings
{
    public const string SectionName = "LLMSettings";
    
    public string ApiKey { get; set; } = default!;
    public string BaseUrl { get; set; } = default!;
    public string Model { get; set; } = "gpt-4";
    public int MaxTokens { get; set; } = 1000;
    public double Temperature { get; set; } = 0.7;
    public int TimeoutSeconds { get; set; } = 30;
    public int MaxRetries { get; set; } = 3;
    public bool EnableCaching { get; set; } = true;
    public int CacheExpirationMinutes { get; set; } = 60;
}
