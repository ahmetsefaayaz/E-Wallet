there is the implementation of E-Wallet microservices project;
<img width="1695" height="749" alt="image" src="https://github.com/user-attachments/assets/39e7c6e5-650f-413f-9497-5049e061a051" />

There are couple of microservices projects in this E-Wallet App(Identity, Account, Notification) with NoSql(MongoDb) and relational databases(Postgresql) communicating from RabbitMq Event Driven Comminucation also there is a reverse proxy gateway (YARP) and Client app.

Whats in this repository

Identity Microservice includes;
ASP.NET Core Minimal API's and dotnet 8.0
CQRS implementation using MediatR library
CQRS Validation Pipeline Behaviors with MediatR and FluentValidation
Use Carter for Minimal API endpoint definition
Cross-cutting concerns Logging, Global Exception Handling and Health Checks
Postgresql database with docker containerization

Account Microservice includes;
Implemented DDD, CQRS and Clean Architecture
Developing CQRS with using MediatR, FluentValidation
Publishes RabbitMQ event queues with using MassTransit-RabbitMQ Configuration
Postgresql database with docker containerization

Notification Microservice includes;
ASP.NET Core Minimal API's and dotnet 8.0
CQRS implementation using MediatR library
CQRS Validation Pipeline Behaviors with MediatR and FluentValidation
Use Carter for Minimal API endpoint definition
Cross-cutting concerns Logging, Global Exception Handling and Health Checks
Consumes IntegrationEvent Queues with using MassTransit and RabbitMQ
MongoDb database with docker containerization


Microservices Communication:
Async Microservices Communication with RabbitMQ Message-Broker Service
Using RabbitMQ Publish/Subscribe Topic Exchange Model
Using MassTransit for abstraction over RabbitMQ Message-Broker system
Create RabbitMQ EventBus.Messages library and add references Microservices

Yarp API Gateway:
Develop API Gateways with Yarp Reverse Proxy applying Gateway Routing Pattern
Yarp Reverse Proxy Configuration; Route, Cluster, Path, Transform, Destinations
Rate Limiting with FixedWindowLimiter on Yarp Reverse Proxy Configuration

Client App:
ASP.NET Core Blazor Web Assembly project
Calls Yarp API gateway with HttpClient

RUN THE PROJECT:
You only need Docker Desktop but you can install any ide and dotnet 8 to launch locally

1- Clone The Repository
2- Start Docker Desktop
3- at the src\E-Wallet root location run the code:
docker compose up --build -d
4- wait for docker to create all containers and thats all.
5- Launch Client App -> http://localhost:6004

<img width="1919" height="970" alt="image" src="https://github.com/user-attachments/assets/410c8a9d-bc52-4f08-ac09-05c4eca4632e" />

