namespace EWallet.Client.Models.Identity;

public sealed class GetUsersResponse
{
    public PaginatedUsers? Users { get; set; }
}

public sealed class PaginatedUsers
{
    public List<User> Data { get; set; } = [];

    public int PageIndex { get; set; }

    public int PageSize { get; set; }

    public long TotalItemCount { get; set; }
}

public sealed class User
{
    public string? UserId { get; set; }

    public string? Email { get; set; }

    public string? UserName { get; set; }
}
