using BuildingBlocks.Messaging.Events;

namespace EmailIntelligence.Application.Events;

public record EmailProcessedEvent(
    string UserId,
    Guid ProcessedEmailId,
    string EmailId,
    EmailPriority Priority,
    EmailCategory Category,
    bool RequiresResponse,
    DateTime ProcessedAt
) : IntegrationEvent;
