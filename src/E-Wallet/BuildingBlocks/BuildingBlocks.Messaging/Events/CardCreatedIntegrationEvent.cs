namespace BuildingBlocks.Messaging.Events;

public class CardCreatedIntegrationEvent: IntegrationEvent
{
    public Guid CardId { get; set; }
    public Guid AccountId { get; set; }
    public string CardHolderName { get; set; } = default!;
    public string CardNumber { get; set; } = default!;
    public string CardExpiryDate { get; set; } = default!;
    public string CardSecurityNumber { get; set; } = default!;
}