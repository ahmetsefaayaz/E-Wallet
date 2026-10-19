using Identity.Api.Dtos;
using Identity.Api.Entities;

namespace Identity.Api.Extensions;

public static class UserExtensions
{
    public static IEnumerable<UserDto> ToUserDtoList(this IEnumerable<User> users)
    {
        return users.Select(user => new UserDto(user.Id.ToString(), user.Email!, user.UserName!));
    }

}