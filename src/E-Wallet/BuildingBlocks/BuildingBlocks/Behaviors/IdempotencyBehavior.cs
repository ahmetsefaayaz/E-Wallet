using BuildingBlocks.CQRS;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Interfaces;
using MediatR;

namespace BuildingBlocks.Behaviors;

public class IdempotencyBehavior<TRequest, TResponse> (IIdempotencyService idempotencyService) 
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IIdempotentCommand<TResponse>
{
    public async Task<TResponse> Handle(TRequest command, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var operationName = typeof(TRequest).Name;
        
        var isAlreadyProcessed = await idempotencyService.RequestExistsAsync(
            command.CurrentUserId, operationName, command.IdempotencyKey, cancellationToken);

        if (isAlreadyProcessed)
        {
            throw new DuplicateRequestException("This request has already been processed.");
        }
        
        idempotencyService.AddRequest(command.CurrentUserId, operationName, command.IdempotencyKey);
        
        return await next();
    }
}