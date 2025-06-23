using BuildingBlocks.CQRS;

namespace EmailIntelligence.Application.Emails.Queries.GetFilteredEmails;

public class GetFilteredEmailsHandler(
    IProcessedEmailRepository emailRepository,
    IEmailDraftRepository draftRepository)
    : IQueryHandler<GetFilteredEmailsQuery, GetFilteredEmailsResult>
{
    public async Task<GetFilteredEmailsResult> Handle(GetFilteredEmailsQuery query, CancellationToken cancellationToken)
    {
        // This is a simplified implementation - in reality you'd want proper pagination and filtering
        var allEmails = await emailRepository.GetByUserIdAsync(query.UserId, cancellationToken);
        
        // Apply filters
        var filteredEmails = allEmails.AsEnumerable();
        
        if (query.Priority.HasValue)
            filteredEmails = filteredEmails.Where(e => e.Priority == query.Priority.Value);
            
        if (query.Category.HasValue)
            filteredEmails = filteredEmails.Where(e => e.Category == query.Category.Value);
            
        if (query.RequiresResponse.HasValue)
            filteredEmails = filteredEmails.Where(e => e.RequiresResponse == query.RequiresResponse.Value);

        // Order by priority and received date
        filteredEmails = filteredEmails
            .OrderByDescending(e => e.Priority)
            .ThenByDescending(e => e.ReceivedAt);

        var totalCount = filteredEmails.Count();
        var pageCount = (int)Math.Ceiling(totalCount / (double)query.PageSize);
        
        var pagedEmails = filteredEmails
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize);

        var emailDtos = new List<FilteredEmailDto>();
        
        foreach (var email in pagedEmails)
        {
            var hasDraft = await draftRepository.GetByProcessedEmailIdAsync(email.Id, cancellationToken) != null;
            
            emailDtos.Add(new FilteredEmailDto(
                email.Id,
                email.EmailId,
                email.Subject,
                email.From,
                email.ReceivedAt,
                email.Priority,
                email.Category,
                email.RequiresResponse,
                email.ConfidenceScore,
                email.ExtractedKeywords,
                email.ActionItems,
                hasDraft
            ));
        }

        return new GetFilteredEmailsResult(emailDtos, totalCount, pageCount);
    }
}
