using BuildingBlocks.Messaging.Events;

namespace EmailIntelligence.Application.Events;

public record DraftEditedEvent(
    string UserId,
    Guid DraftId,
    string OriginalContent,
    string EditedContent,
    string[] EditTypes,
    DateTime EditedAt
) : IntegrationEvent;
