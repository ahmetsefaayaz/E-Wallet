using BuildingBlocks.CQRS;
using Identity.Api.Data;
using Identity.Api.Dtos;
using Microsoft.EntityFrameworkCore;

namespace Identity.Api.Identity.Queries.GetMyAddress;

public record GetMyAddressQuery(Guid UserId) : IQuery<GetMyAddressResult>;

public record GetMyAddressResult(AddressDto Address);
internal sealed class GetMyAddressQueryHandler(IdentityServiceDbContext dbContext)
    : IQueryHandler<GetMyAddressQuery, GetMyAddressResult>
{
    public async Task<GetMyAddressResult> Handle(GetMyAddressQuery query, CancellationToken cancellationToken)
    {
        
        var user = await dbContext.Users
                       .AsNoTracking()
                       .FirstOrDefaultAsync(u => u.Id == query.UserId, cancellationToken)
                   ?? throw new Exception($"User with ID {query.UserId} not found.");

        if (user.Address == null)
        {
            return new GetMyAddressResult(null!);
        }

        var addressDto = new AddressDto
        {
            AddressLine = user.Address.AddressLine,
            City = user.Address.City,
            State = user.Address.State,
            Country = user.Address.Country,
            ZipCode = user.Address.ZipCode
        };

        return new GetMyAddressResult(addressDto);
    }
}