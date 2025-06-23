namespace EmailIntelligence.API.Drafts;

public class DraftEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/drafts/generate", async (GenerateDraftRequest request, ISender sender) =>
        {
            var command = new GenerateDraftCommand(
                request.ProcessedEmailId,
                request.UserId,
                request.AdditionalContext
            );

            var result = await sender.Send(command);
            
            return Results.Ok(new GenerateDraftResponse(
                result.DraftId,
                result.GeneratedContent,
                result.ConfidenceScore
            ));
        })
        .WithName("GenerateDraft")
        .WithSummary("Generate an AI draft response for an email")
        .WithDescription("Creates a draft response using AI based on the original email and user's tone profile")
        .Produces<GenerateDraftResponse>(StatusCodes.Status200OK)
        .ProducesValidationProblem()
        .WithTags("Drafts");

        app.MapPut("/drafts/{draftId}/edit", async (Guid draftId, EditDraftRequest request, ISender sender) =>
        {
            var command = new EditDraftCommand(
                draftId,
                request.UserId,
                request.EditedContent,
                request.EditTypes
            );

            var result = await sender.Send(command);
            
            return Results.Ok(new EditDraftResponse(result.DraftId, result.Success));
        })
        .WithName("EditDraft")
        .WithSummary("Edit a generated draft")
        .WithDescription("Records user edits to a draft for continuous learning and improvement")
        .Produces<EditDraftResponse>(StatusCodes.Status200OK)
        .ProducesValidationProblem()
        .WithTags("Drafts");
    }
}

public record GenerateDraftRequest(
    Guid ProcessedEmailId,
    string UserId,
    string? AdditionalContext = null
);

public record GenerateDraftResponse(
    Guid DraftId,
    string GeneratedContent,
    double ConfidenceScore
);

public record EditDraftRequest(
    string UserId,
    string EditedContent,
    string[] EditTypes
);

public record EditDraftResponse(
    Guid DraftId,
    bool Success
);
