using System.Reflection;
using System.Text;
using BuildingBlocks.Behaviors;
using BuildingBlocks.Exceptions.Handlers;
using BuildingBlocks.Messaging.MassTransit;
using Carter;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Notification.Api.Data;
using Notification.Api.Interfaces;
using Notification.Api.Mail;

var builder = WebApplication.CreateBuilder(args);
BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
builder.Services.AddMessageBroker(builder.Configuration, Assembly.GetExecutingAssembly());
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddCarter();



builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    var validAudiences = builder.Configuration.GetSection("Jwt:Audiences").Get<string[]>();
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudiences = validAudiences,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
});

builder.Services.AddAuthorization(opt =>
{
    opt.AddPolicy("IsAdmin", policy => policy.RequireRole("Admin"));
            
    opt.AddPolicy("NotificationDepartment", policy => policy.RequireAssertion(context =>
        context.User.HasClaim("Department", "Notification") || context.User.IsInRole("Admin")));
});



builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
    cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
});

builder.Services.AddTransient<IEmailService, EmailService>();

var app = builder.Build();

app.UseExceptionHandler(opt => { });

app.UseAuthentication();
app.UseAuthorization();
app.MapCarter();


app.Run();