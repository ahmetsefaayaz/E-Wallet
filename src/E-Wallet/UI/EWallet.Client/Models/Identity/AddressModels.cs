namespace EWallet.Client.Models.Identity;

public sealed class AddressDto
{
    public string AddressLine { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string State { get; set; } = string.Empty;

    public string Country { get; set; } = string.Empty;

    public string ZipCode { get; set; } = string.Empty;
}

public sealed class CreateAddressRequest
{
    public AddressDto Address { get; set; } = new();
}

public sealed class CreateAddressResponse
{
    public bool IsSuccess { get; set; }
}

public sealed class UpdateAddressRequest
{
    public AddressDto Address { get; set; } = new();
}

public sealed class UpdateAddressResponse
{
    public bool IsSuccess { get; set; }
}
