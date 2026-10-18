using Identity.Api;
using Identity.Api.Data.SeedData;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddIdentityServices(builder.Configuration);


var app = builder.Build();

app.UseIdentityServices();

if (app.Environment.IsDevelopment())
{
    await app.Initialize();
}

app.Run();