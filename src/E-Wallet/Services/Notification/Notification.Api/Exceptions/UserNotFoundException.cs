using BuildingBlocks.Exceptions;

namespace Notification.Api.Exceptions;

public class UserNotFoundException: NotFoundException
{
    public UserNotFoundException(Guid userId) : base("User", userId)
    {
    }
}