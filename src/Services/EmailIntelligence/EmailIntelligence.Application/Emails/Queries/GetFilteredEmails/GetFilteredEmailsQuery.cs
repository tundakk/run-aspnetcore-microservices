using BuildingBlocks.CQRS;

namespace EmailIntelligence.Application.Emails.Queries.GetFilteredEmails;

public record GetFilteredEmailsQuery(
    string UserId,
    EmailPriority? Priority = null,
    EmailCategory? Category = null,
    bool? RequiresResponse = null,
    int Page = 1,
    int PageSize = 20
) : IQuery<GetFilteredEmailsResult>;

public record GetFilteredEmailsResult(
    IEnumerable<FilteredEmailDto> Emails,
    int TotalCount,
    int PageCount
);

public record FilteredEmailDto(
    Guid Id,
    string EmailId,
    string Subject,
    string From,
    DateTime ReceivedAt,
    EmailPriority Priority,
    EmailCategory Category,
    bool RequiresResponse,
    double ConfidenceScore,
    string[] Keywords,
    string? ActionItems,
    bool HasDraft
);
