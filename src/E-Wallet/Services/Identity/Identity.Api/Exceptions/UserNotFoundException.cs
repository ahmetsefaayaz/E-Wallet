using BuildingBlocks.Exceptions;

namespace Identity.Api.Exceptions;

public class UserNotFoundException:NotFoundException
{

    public UserNotFoundException(string email) : base("User", email)
    {
    }
}