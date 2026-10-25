using System.Reflection;
using BuildingBlocks.Messaging.MassTransit;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Notification.Api.Data;
using Notification.Api.Interfaces;

var builder = WebApplication.CreateBuilder(args);
BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
builder.Services.AddMessageBroker(builder.Configuration, Assembly.GetExecutingAssembly());
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();

var app = builder.Build();



app.Run();