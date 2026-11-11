namespace BuildingBlocks.CQRS;

public interface IIdempotentCommand<out TResponse> : ICommand<TResponse>
{
    Guid CurrentUserId { get; }
    string IdempotencyKey { get; }
}