using EmailIntelligence.Infrastructure.Configuration;
using EmailIntelligence.Infrastructure.Services;
using EmailIntelligence.Infrastructure.Data;
using EmailIntelligence.Infrastructure.Repositories;
using EmailIntelligence.Domain.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

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
        });        services.AddHttpClient<IDraftGenerationService, DraftGenerationService>(client =>
        {
            var llmSettings = configuration.GetSection(LLMSettings.SectionName).Get<LLMSettings>()!;
            client.BaseAddress = new Uri(llmSettings.BaseUrl);
            client.Timeout = TimeSpan.FromSeconds(llmSettings.TimeoutSeconds);
        });

        // Database
        services.AddDbContext<EmailIntelligenceDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Database")));

        // Application Services
        services.AddScoped<IEmailAnalysisService, EmailAnalysisService>();
        services.AddScoped<IDraftGenerationService, DraftGenerationService>();
        services.AddScoped<ILearningService, LearningService>();

        // Repositories
        services.AddScoped<IProcessedEmailRepository, ProcessedEmailRepository>();
        services.AddScoped<IEmailDraftRepository, EmailDraftRepository>();
        services.AddScoped<IUserToneProfileRepository, UserToneProfileRepository>();

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
