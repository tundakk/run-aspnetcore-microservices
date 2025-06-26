using EmailIntelligence.Application.Services;
using EmailIntelligence.Infrastructure.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System.Text.Json;

namespace EmailIntelligence.Infrastructure.Services;

public class SemanticKernelService : ISemanticKernelService
{
    private readonly Kernel _kernel;
    private readonly ILogger<SemanticKernelService> _logger;
    private readonly LLMSettings _settings;

    public SemanticKernelService(
        IOptions<LLMSettings> settings,
        ILogger<SemanticKernelService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
        
        _kernel = CreateKernel();
    }

    private Kernel CreateKernel()
    {
        var builder = Kernel.CreateBuilder();
        
        builder.AddOpenAIChatCompletion(
            _settings.Model,
            _settings.ApiKey,
            httpClient: new HttpClient { BaseAddress = new Uri(_settings.BaseUrl) });

        return builder.Build();
    }

    public Kernel GetKernel() => _kernel;

    public async Task<string> GenerateEmailDraftAsync(
        string subject, 
        string description, 
        string? userTone = null, 
        string? contextualHistory = null, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var chatService = _kernel.GetRequiredService<IChatCompletionService>();
            
            var prompt = BuildEmailDraftPrompt(subject, description, userTone, contextualHistory);
            var chatHistory = new ChatHistory();
            chatHistory.AddSystemMessage("Du är en expert på att skriva professionella och personliga e-postmeddelanden på svenska. Du förstår svenska affärskultur och kommunikationsstilar.");
            chatHistory.AddUserMessage(prompt);

            var executionSettings = new OpenAIPromptExecutionSettings
            {
                MaxTokens = _settings.MaxTokens,
                Temperature = _settings.Temperature
            };

            var response = await chatService.GetChatMessageContentAsync(
                chatHistory, 
                executionSettings, 
                cancellationToken: cancellationToken);

            return response.Content ?? "";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating email draft for subject: {Subject}", subject);
            throw;
        }
    }

    public async Task<string> ClassifyEmailAsync(
        string subject, 
        string body, 
        string from, 
        string? contextualHistory = null, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var chatService = _kernel.GetRequiredService<IChatCompletionService>();
            
            var prompt = BuildClassificationPrompt(subject, body, from, contextualHistory);
            var chatHistory = new ChatHistory();
            chatHistory.AddSystemMessage("Du är en expert på att analysera och kategorisera e-postmeddelanden. Svara endast med giltig JSON.");
            chatHistory.AddUserMessage(prompt);

            var executionSettings = new OpenAIPromptExecutionSettings
            {
                MaxTokens = 500,
                Temperature = 0.1 // Lower temperature for classification
            };

            var response = await chatService.GetChatMessageContentAsync(
                chatHistory,
                executionSettings,
                cancellationToken: cancellationToken);

            return response.Content ?? "";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error classifying email from: {From}", from);
            throw;
        }
    }

    public async Task<string> AnalyzeToneAsync(string content, CancellationToken cancellationToken = default)
    {
        try
        {
            var chatService = _kernel.GetRequiredService<IChatCompletionService>();
            
            var prompt = $@"
Analysera tonen i följande text och beskriv den i JSON-format:

Text: {content}

Svara med följande JSON-struktur:
{{
    ""overall_tone"": ""professionell|vänlig|formell|informell|neutral|entusiastisk|bekymrad"",
    ""formality_level"": 1-5,
    ""emotional_intensity"": 1-5,
    ""key_characteristics"": [""karakteristik1"", ""karakteristik2""],
    ""swedish_cultural_notes"": ""kommentar om svensk kommunikationsstil""
}}";

            var chatHistory = new ChatHistory();
            chatHistory.AddSystemMessage("Du är en expert på svensk kommunikation och tonanalys.");
            chatHistory.AddUserMessage(prompt);

            var response = await chatService.GetChatMessageContentAsync(chatHistory, cancellationToken: cancellationToken);
            return response.Content ?? "";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analyzing tone for content");
            throw;
        }
    }

    public async Task<string> ExtractKeywordsAsync(string content, CancellationToken cancellationToken = default)
    {
        try
        {
            var chatService = _kernel.GetRequiredService<IChatCompletionService>();
            
            var prompt = $@"
Extrahera de viktigaste nyckelorden från följande text på svenska:

Text: {content}

Svara med JSON-format:
{{
    ""keywords"": [""nyckelord1"", ""nyckelord2"", ""nyckelord3""],
    ""entities"": [""person"", ""företag"", ""plats""],
    ""topics"": [""ämne1"", ""ämne2""],
    ""priority_indicators"": [""brådskande"", ""viktigt""]
}}";

            var chatHistory = new ChatHistory();
            chatHistory.AddSystemMessage("Du är en expert på svensk textanalys och nyckelordsextraktion.");
            chatHistory.AddUserMessage(prompt);

            var response = await chatService.GetChatMessageContentAsync(chatHistory, cancellationToken: cancellationToken);
            return response.Content ?? "";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting keywords from content");
            throw;
        }
    }

    public async Task<string> SummarizeContentAsync(string content, int maxWords = 100, CancellationToken cancellationToken = default)
    {
        try
        {
            var chatService = _kernel.GetRequiredService<IChatCompletionService>();
            
            var prompt = $@"
Sammanfatta följande text på svenska i max {maxWords} ord:

Text: {content}

Fokusera på:
- Huvudbudskapet
- Viktiga handlingsåtgärder
- Deadlines eller viktiga datum
- Kontaktinformation

Sammanfattning:";

            var chatHistory = new ChatHistory();
            chatHistory.AddSystemMessage("Du är en expert på att sammanfatta svensk text koncist och tydligt.");
            chatHistory.AddUserMessage(prompt);

            var response = await chatService.GetChatMessageContentAsync(chatHistory, cancellationToken: cancellationToken);
            return response.Content ?? "";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error summarizing content");
            throw;
        }
    }

    public Task<T> ExecuteSemanticFunctionAsync<T>(string functionName, object parameters, CancellationToken cancellationToken = default)
    {
        try
        {
            // This is a placeholder for executing custom semantic functions
            // You can implement specific semantic functions here
            throw new NotImplementedException("Custom semantic functions not yet implemented");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing semantic function: {FunctionName}", functionName);
            throw;
        }
    }

    private string BuildEmailDraftPrompt(string subject, string description, string? userTone, string? contextualHistory)
    {
        var prompt = $@"
Skriv ett professionellt e-postmeddelande på svenska baserat på följande information:

Ämne: {subject}
Beskrivning: {description}";

        if (!string.IsNullOrEmpty(userTone))
        {
            prompt += $@"

Användarens önskade ton: {userTone}
Anpassa svaret för att matcha denna kommunikationsstil.";
        }

        if (!string.IsNullOrEmpty(contextualHistory))
        {
            prompt += $@"

Tidigare konversationshistorik:
{contextualHistory}

Ta hänsyn till denna kontext när du skriver svaret.";
        }

        prompt += @"

Riktlinjer:
- Skriv ett komplett e-postmeddelande med hälsning och avslutning
- Använd lämplig svensk affärsetiketti
- Var tydlig och koncis
- Anpassa formalisering baserat på kontext
- Inkludera relevanta handlingsåtgärder om nödvändigt

E-postmeddelande:";

        return prompt;
    }

    private string BuildClassificationPrompt(string subject, string body, string from, string? contextualHistory)
    {
        var prompt = $@"
Analysera detta e-postmeddelande och klassificera det i JSON-format:

Ämne: {subject}
Från: {from}
Meddelande: {body}";

        if (!string.IsNullOrEmpty(contextualHistory))
        {
            prompt += $@"

Tidigare konversationshistorik:
{contextualHistory}";
        }

        prompt += @"

Svara med följande JSON-struktur:
{
    ""priority"": 0-3 (0=Låg, 1=Medium, 2=Hög, 3=Kritisk),
    ""category"": ""RequiresResponse|Informational|ActionRequired|Meeting|Support|Marketing|Newsletter|Spam|Personal|Internal"",
    ""requiresResponse"": true/false,
    ""confidenceScore"": 0.0-1.0,
    ""keywords"": [""nyckelord1"", ""nyckelord2""],
    ""actionItems"": ""beskrivning av nödvändiga åtgärder eller null"",
    ""sentiment"": ""positive|neutral|negative"",
    ""urgency_indicators"": [""brådskande"", ""asap"", ""deadline""],
    ""swedish_context"": ""kommentar om svensk kommunikationskontext""
}

Tänk på:
- Svenska affärskonventioner
- Avsändarens betydelse
- Innehållets typ och brådska
- Mötesinbjudningar eller kalenderposter
- Support-ärenden eller problem";

        return prompt;
    }
}
