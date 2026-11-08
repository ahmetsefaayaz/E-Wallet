using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Authorization;
using EWallet.Client;
using EWallet.Client.Services.Auth;
using EWallet.Client.Services.Accounts;
using EWallet.Client.Services.Identity;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");



builder.Services.AddScoped<TokenStorage>();
builder.Services.AddScoped<JwtAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(
    sp => sp.GetRequiredService<JwtAuthenticationStateProvider>());
builder.Services.AddAuthorizationCore(options =>
{
    options.AddPolicy("IdentityDepartment", policy =>
    {
        policy.RequireAssertion(context =>
            context.User.HasClaim(
                "Department",
            "Identity") ||
            context.User.IsInRole("Admin"));
    });
    options.AddPolicy("AccountDepartment", policy =>
    {
        policy.RequireAssertion(context =>
        context.User.HasClaim(
            "Department",
            "Account") ||
        context.User.IsInRole("Admin"));
    });
});
builder.Services.AddScoped<TokenAuthenticationHandler>();
builder.Services.AddScoped<AuthApiClient>();
builder.Services.AddScoped<AccountApiClient>();
builder.Services.AddScoped<UsersApiClient>();
builder.Services.AddScoped<IdentityAdministrationApiClient>();
builder.Services.AddScoped(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var baseUrl = configuration["ApiGateway:BaseUrl"];

    if (string.IsNullOrWhiteSpace(baseUrl))
    {
        throw new InvalidOperationException(
            "The API gateway URL is not configured.");
    }

    var tokenHandler = sp.GetRequiredService<TokenAuthenticationHandler>();

    tokenHandler.InnerHandler = new HttpClientHandler();

    return new HttpClient(tokenHandler)
    {
        BaseAddress = new Uri(baseUrl)
    };
});

await builder.Build().RunAsync();