namespace EmailIntelligence.API.Emails;

public class ProcessEmailEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/emails/process", async (ProcessEmailRequest request, ISender sender) =>
        {
            var command = new ProcessEmailCommand(
                request.EmailId,
                request.UserId,
                request.Subject,
                request.From,
                request.To,
                request.Body,
                request.ReceivedAt
            );

            var result = await sender.Send(command);
            
            return Results.Ok(new ProcessEmailResponse(
                result.ProcessedEmailId,
                result.Priority.ToString(),
                result.Category.ToString(),
                result.RequiresResponse,
                result.ConfidenceScore,
                result.Keywords,
                result.ActionItems
            ));
        })
        .WithName("ProcessEmail")
        .WithSummary("Process and categorize an incoming email")
        .WithDescription("Analyzes an email using AI to determine priority, category, and required actions")
        .Produces<ProcessEmailResponse>(StatusCodes.Status200OK)
        .ProducesValidationProblem()
        .WithTags("Emails");

        app.MapGet("/emails/filtered", async (
            [AsParameters] GetFilteredEmailsRequest request, 
            ISender sender) =>
        {
            var query = new GetFilteredEmailsQuery(
                request.UserId,
                request.Priority,
                request.Category,
                request.RequiresResponse,
                request.Page,
                request.PageSize
            );

            var result = await sender.Send(query);
            
            return Results.Ok(new GetFilteredEmailsResponse(
                result.Emails.Select(e => new FilteredEmailDto(
                    e.Id,
                    e.EmailId,
                    e.Subject,
                    e.From,
                    e.ReceivedAt,
                    e.Priority,
                    e.Category,
                    e.RequiresResponse,
                    e.ConfidenceScore,
                    e.Keywords,
                    e.ActionItems,
                    e.HasDraft
                )),
                result.TotalCount,
                result.PageCount
            ));
        })
        .WithName("GetFilteredEmails")
        .WithSummary("Get filtered and categorized emails")
        .WithDescription("Retrieves emails filtered by priority, category, or response requirements")
        .Produces<GetFilteredEmailsResponse>(StatusCodes.Status200OK)
        .WithTags("Emails");
    }
}

public record ProcessEmailRequest(
    string EmailId,
    string UserId,
    string Subject,
    string From,
    string To,
    string Body,
    DateTime ReceivedAt
);

public record ProcessEmailResponse(
    Guid ProcessedEmailId,
    string Priority,
    string Category,
    bool RequiresResponse,
    double ConfidenceScore,
    string[] Keywords,
    string? ActionItems
);

public record GetFilteredEmailsRequest(
    string UserId,
    EmailIntelligence.Domain.Enums.EmailPriority? Priority = null,
    EmailIntelligence.Domain.Enums.EmailCategory? Category = null,
    bool? RequiresResponse = null,
    int Page = 1,
    int PageSize = 20
);

public record GetFilteredEmailsResponse(
    IEnumerable<FilteredEmailDto> Emails,
    int TotalCount,
    int PageCount
);
