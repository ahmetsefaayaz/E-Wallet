namespace Identity.Api.Dtos;

public record UserDto(
    string UserId,
    string Email,
    string UserName);