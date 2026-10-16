using Identity.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddIdentityServices(builder.Configuration);


var app = builder.Build();

app.UseIdentityServices();

app.Run();