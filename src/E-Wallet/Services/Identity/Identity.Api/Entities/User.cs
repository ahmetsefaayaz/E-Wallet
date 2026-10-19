using Microsoft.AspNetCore.Identity;

namespace Identity.Api.Entities;

public class User:IdentityUser<Guid>
{
    public Address? Address { get; private set; }

    public void SetAddress(Address address)
    {
        Address = address;
    }
}