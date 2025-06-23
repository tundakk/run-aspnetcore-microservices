using EmailIntelligence.Infrastructure.Configuration;
using EmailIntelligence.Infrastructure.Services;

namespace EmailIntelligence.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Configuration
        services.Configure<LLMSettings>(
            configuration.GetSection(LLMSettings.SectionName));

        // HTTP Client for LLM API
        services.AddHttpClient<IEmailAnalysisService, EmailAnalysisService>(client =>
        {
            var llmSettings = configuration.GetSection(LLMSettings.SectionName).Get<LLMSettings>()!;
            client.BaseAddress = new Uri(llmSettings.BaseUrl);
            client.Timeout = TimeSpan.FromSeconds(llmSettings.TimeoutSeconds);
        });

        services.AddHttpClient<IDraftGenerationService, DraftGenerationService>(client =>
        {
            var llmSettings = configuration.GetSection(LLMSettings.SectionName).Get<LLMSettings>()!;
            client.BaseAddress = new Uri(llmSettings.BaseUrl);
            client.Timeout = TimeSpan.FromSeconds(llmSettings.TimeoutSeconds);
        });

        // Application Services
        services.AddScoped<IEmailAnalysisService, EmailAnalysisService>();
        services.AddScoped<IDraftGenerationService, DraftGenerationService>();
        services.AddScoped<ILearningService, LearningService>();

        // TODO: Add database repositories
        // services.AddScoped<IProcessedEmailRepository, ProcessedEmailRepository>();
        // services.AddScoped<IEmailDraftRepository, EmailDraftRepository>();
        // services.AddScoped<IUserToneProfileRepository, UserToneProfileRepository>();

        // Health Checks
        services.AddHealthChecks()
            .AddCheck("llm-api", () => 
            {
                // Add health check for LLM API
                return Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy();
            });

        return services;
    }
}
