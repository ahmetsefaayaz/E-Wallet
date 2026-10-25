namespace BuildingBlocks.Messaging.Events;

public class TransactionCreatedIntegrationEvent: IntegrationEvent
{
    public Guid TransactionId { get; set; }
    public Guid AccountId { get; set; }
    public Guid? CardId { get; set; }
    public string TransactionType { get; set; } = default!;
    public decimal Amount { get; set; }
}