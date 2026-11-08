namespace EWallet.Client.Models.Identity;

public sealed class CreateClaimRequest
{
    public string Type { get; set; } = string.Empty;

    public string Value { get; set; } = string.Empty;
}

public sealed class CreateClaimResponse
{
    public bool IsSuccess { get; set; }
}

public sealed class CreateRoleRequest
{
    public string RoleName { get; set; } = string.Empty;
}

public sealed class CreateRoleResponse
{
    public bool IsSuccess { get; set; }
}

public sealed class AddRoleRequest
{
    public string RoleName { get; set; } = string.Empty;
}

public sealed class AddRoleResponse
{
    public bool IsSuccess { get; set; }
}
