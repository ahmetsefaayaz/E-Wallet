using System.Reflection;
using BuildingBlocks.Behaviors;
using BuildingBlocks.Exceptions.Handlers;
using BuildingBlocks.Messaging.MassTransit;
using Carter;
using FluentValidation;
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
app.MapCarter();


app.Run();