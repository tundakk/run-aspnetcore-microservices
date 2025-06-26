using EmailIntelligence.Application.Services;
using EmailIntelligence.Infrastructure.Configuration;
using EmailIntelligence.Infrastructure.Services;
using EmailIntelligence.Infrastructure.Data;
using EmailIntelligence.Infrastructure.Repositories;
using EmailIntelligence.Domain.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel;
using Pgvector.EntityFrameworkCore;

#pragma warning disable SKEXP0010 // Type is for evaluation purposes only and is subject to change or removal in future updates.

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

        var llmSettings = configuration.GetSection(LLMSettings.SectionName).Get<LLMSettings>()!;

        // Semantic Kernel Services
        services.AddSingleton<Kernel>(serviceProvider =>
        {
            var kernelBuilder = Kernel.CreateBuilder();
            kernelBuilder.AddOpenAIChatCompletion(
                llmSettings.Model,
                llmSettings.ApiKey,
                httpClient: new HttpClient { BaseAddress = new Uri(llmSettings.BaseUrl) });
            return kernelBuilder.Build();
        });

        // OpenAI Embedding Service
        services.AddSingleton<OpenAITextEmbeddingGenerationService>(serviceProvider =>
        {
            return new OpenAITextEmbeddingGenerationService(
                llmSettings.EmbeddingModel,
                llmSettings.ApiKey,
                httpClient: new HttpClient { BaseAddress = new Uri(llmSettings.BaseUrl) });
        });

        // Database
        services.AddDbContext<EmailIntelligenceDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("Database"), o =>
            {
                // Enable vector extension for PostgreSQL
                o.UseVector();
            });
        });

        // HTTP Clients
        services.AddHttpClient<IDraftGenerationService, DraftGenerationService>();
        services.AddHttpClient<ILearningService, LearningService>();

        // Application Services  
        services.AddScoped<ISemanticKernelService, SemanticKernelService>();
        services.AddScoped<IEmbeddingService, EmbeddingService>();
        services.AddScoped<IEmailAnalysisService, EmailAnalysisService>();
        services.AddScoped<IDraftGenerationService, DraftGenerationService>();
        services.AddScoped<ILearningService, LearningService>();

        // Repositories
        services.AddScoped<IProcessedEmailRepository, ProcessedEmailRepository>();
        services.AddScoped<IEmailDraftRepository, EmailDraftRepository>();
        services.AddScoped<IUserToneProfileRepository, UserToneProfileRepository>();
        services.AddScoped<IEmailEmbeddingRepository, EmailEmbeddingRepository>();
        services.AddScoped<ILearningPatternRepository, LearningPatternRepository>();

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
