using EmailIntelligence.Application.Services;
using EmailIntelligence.Domain.Entities;
using EmailIntelligence.Application.Drafts.Commands.GenerateDraft;
using EmailIntelligence.Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text;

namespace EmailIntelligence.Infrastructure.Services;

public class DraftGenerationService(
    HttpClient httpClient,
    IOptions<LLMSettings> llmSettings,
    ILogger<DraftGenerationService> logger) : IDraftGenerationService
{
    private readonly LLMSettings _settings = llmSettings.Value;

    public async Task<DraftGenerationResult> GenerateDraftAsync(
        string originalSubject,
        string originalBody,
        string from,
        UserToneProfile? toneProfile,
        string? additionalContext = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var prompt = BuildDraftPrompt(originalSubject, originalBody, from, toneProfile, additionalContext);
            var response = await CallLLMAsync(prompt, cancellationToken);
            
            return new DraftGenerationResult(
                response,
                _settings.Model,
                prompt,
                0.8, // Default confidence
                0.7  // Default quality score
            );
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error generating draft response");
            
            // Fallback to simple template
            var fallbackContent = GenerateFallbackDraft(originalSubject, originalBody);
            return new DraftGenerationResult(
                fallbackContent,
                "fallback",
                "template-based",
                0.3,
                0.5
            );
        }
    }

    private string BuildDraftPrompt(
        string originalSubject, 
        string originalBody, 
        string from, 
        UserToneProfile? toneProfile,
        string? additionalContext)
    {
        var toneGuidance = toneProfile != null 
            ? $@"
User's tone profile:
- Writing style: {toneProfile.WritingStyle}
- Confidence level: {toneProfile.ConfidenceLevel:P0}
- Preferred phrases: {toneProfile.PreferredPhrases}
- Avoided phrases: {toneProfile.AvoidedPhrases}

Please match this user's established tone and style."
            : "Use a professional but friendly tone.";

        var contextSection = !string.IsNullOrEmpty(additionalContext)
            ? $"\nAdditional context: {additionalContext}"
            : "";

        return $@"
You are Garba's AI assistant helping to draft email responses. Generate a professional email response.

Original email:
Subject: {originalSubject}
From: {from}
Body: {originalBody}

{toneGuidance}
{contextSection}

Instructions:
1. Draft a clear, concise response
2. Address all questions or points raised
3. Maintain professionalism while matching the user's tone
4. Include appropriate greetings and closings
5. Be helpful and solution-oriented
6. Keep response length appropriate to the original email

Generate ONLY the email body content (no subject line, no headers):
";
    }

    private async Task<string> CallLLMAsync(string prompt, CancellationToken cancellationToken)
    {
        var requestBody = new
        {
            model = _settings.Model,
            messages = new[]
            {
                new { role = "system", content = "You are Garba's professional email assistant. Write clear, helpful email responses." },
                new { role = "user", content = prompt }
            },
            max_tokens = _settings.MaxTokens,
            temperature = _settings.Temperature
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        httpClient.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _settings.ApiKey);

        var response = await httpClient.PostAsync("/v1/chat/completions", content, cancellationToken);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        var responseObj = JsonSerializer.Deserialize<JsonElement>(responseContent);
        
        return responseObj
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString() ?? "";
    }

    private string GenerateFallbackDraft(string originalSubject, string originalBody)
    {
        return $@"Thank you for your email regarding ""{originalSubject}"".

I have received your message and will review the details you've provided. I'll get back to you with a response as soon as possible.

If you have any urgent questions in the meantime, please don't hesitate to reach out.

Best regards,
Garba";
    }
}
