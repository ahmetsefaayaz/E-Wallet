namespace Identity.Api.Identity.Queries.GetUserId;

public interface IGetUserId
{
    Task<Guid> GetUserIdFromContext();
}