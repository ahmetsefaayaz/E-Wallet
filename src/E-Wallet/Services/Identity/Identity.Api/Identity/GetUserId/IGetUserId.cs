namespace Identity.Api.Identity.GetUserId;

public interface IGetUserId
{
    Task<Guid> GetUserIdFromContext();
}