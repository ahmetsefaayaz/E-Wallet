using BuildingBlocks.Exceptions;

namespace Identity.Api.Exceptions;

public class UserNotFoundException:NotFoundException
{
    public UserNotFoundException(): base("User not found")
    {
        
    }
    public UserNotFoundException(string email) : base("User", email)
    {
    }
}