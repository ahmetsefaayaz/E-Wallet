namespace Identity.Api.Dtos;

public record AddressDto
{
    public string AddressLine { get; init; }
    public string City { get; init; }
    public string State { get; init; }
    public string Country { get; init; }
    public string ZipCode { get; init; }
}