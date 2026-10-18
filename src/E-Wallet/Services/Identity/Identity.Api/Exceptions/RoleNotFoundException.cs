using BuildingBlocks.Exceptions;

namespace Identity.Api.Exceptions;

public class RoleNotFoundException: NotFoundException
{
    public RoleNotFoundException(): base("Role not found")
    {
    }

    public RoleNotFoundException(string rolename) : base("Role", rolename)
    {
    }
}