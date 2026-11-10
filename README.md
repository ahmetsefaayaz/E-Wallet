<div align="center">

# E-Wallet Microservices

**An event-driven E-Wallet microservices project built with .NET 8, Blazor WebAssembly, YARP API Gateway, RabbitMQ, PostgreSQL, and MongoDB.**

[![.NET 8.0](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![Blazor](https://img.shields.io/badge/Blazor_WASM-512BD4?style=for-the-badge&logo=blazor&logoColor=white)](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL_17-4169E1?style=for-the-badge&logo=postgresql&logoColor=white)](https://www.postgresql.org/)
[![MongoDB](https://img.shields.io/badge/MongoDB-47A248?style=for-the-badge&logo=mongodb&logoColor=white)](https://www.mongodb.com/)
[![RabbitMQ](https://img.shields.io/badge/RabbitMQ-FF6600?style=for-the-badge&logo=rabbitmq&logoColor=white)](https://www.rabbitmq.com/)
[![Docker](https://img.shields.io/badge/Docker_Compose-2496ED?style=for-the-badge&logo=docker&logoColor=white)](https://www.docker.com/)

</div>

---

## Overview

This repository contains an E-Wallet application built with a microservices architecture. It includes three backend services (**Identity**, **Account**, and **Notification**), relational (**PostgreSQL**) and NoSQL (**MongoDB**) databases, event-driven communication with **RabbitMQ**, a reverse proxy gateway (**YARP**), and a **Blazor WebAssembly** client app.

<p align="center">
  <img width="1695" height="749" alt="E-Wallet Microservices Architecture" src="https://github.com/user-attachments/assets/39e7c6e5-650f-413f-9497-5049e061a051" />
</p>

---

## Tech Stack

| Category | Technologies |
| :--- | :--- |
| **Framework** | .NET 8.0, ASP.NET Core Minimal APIs, Blazor WebAssembly |
| **Architecture** | Microservices, Domain-Driven Design (DDD), Clean Architecture, Vertical Slice, CQRS |
| **API & Gateway** | Carter, YARP Reverse Proxy, Rate Limiting (`FixedWindowLimiter`), Swagger / OpenAPI |
| **Libraries** | MediatR, FluentValidation, Mapster |
| **Messaging** | RabbitMQ, MassTransit |
| **Databases** | PostgreSQL 17, Entity Framework Core 8, MongoDB |
| **Authentication** | ASP.NET Core Identity, JWT Bearer Authentication, Role & Claim Authorization |
| **Email** | MailKit (SMTP), MailHog |
| **Containers** | Docker, Docker Compose, Nginx |

---

## What's in This Repository

### Identity Microservice (`Identity.Api`)
Handles user registration, login, roles, claims, and user addresses.
- ASP.NET Core Minimal APIs with .NET 8.0
- Uses **Carter** for Minimal API endpoints
- CQRS implementation using **MediatR**
- Validation and logging pipelines with **MediatR** and **FluentValidation**
- Authentication and authorization with **ASP.NET Core Identity** and **JWT tokens**
- Publishes `UserRegisteredIntegrationEvent` to RabbitMQ when a user registers
- Global exception handling and automatic database seeding
- **PostgreSQL** database running in Docker

### Account Microservice (`Account.Api`, `Account.Application`, `Account.Domain`, `Account.Infrastructure`)
Manages user wallet accounts, cards, balances, and money transactions.
- Built with **DDD**, **CQRS**, and **Clean Architecture**
- Uses **Carter** for Minimal API endpoints
- CQRS commands and queries with **MediatR** and **FluentValidation**
- Domain models (`Account`, `Card`, `Transaction`), Value Objects, and Domain Events
- Supports creating/deleting accounts, managing cards, and running transactions (**Deposit**, **Withdraw**, and **Transfer**)
- Publishes `CardCreatedIntegrationEvent` and `TransactionCreatedIntegrationEvent` to RabbitMQ using **MassTransit**
- **PostgreSQL** database running in Docker with EF Core migrations and seed data

### Notification Microservice (`Notification.Api`)
Listens for events from other services, sends emails, and saves user notifications.
- ASP.NET Core Minimal APIs with .NET 8.0
- Uses **Carter** for Minimal API endpoints
- CQRS implementation with **MediatR** and **FluentValidation**
- Global exception handling and logging behaviors
- Consumes integration events (`UserRegisteredIntegrationEvent`, `CardCreatedIntegrationEvent`, `TransactionCreatedIntegrationEvent`) from **RabbitMQ** using **MassTransit**
- Sends emails through **MailKit** and **MailHog**
- **MongoDB** database running in Docker

### BuildingBlocks (`BuildingBlocks` & `BuildingBlocks.Messaging`)
Shared class libraries used by the microservices:
- **`BuildingBlocks`**: Base CQRS interfaces (`ICommand`, `IQuery`), MediatR validation and logging behaviors, global exception handling, and pagination helpers.
- **`BuildingBlocks.Messaging`**: Shared RabbitMQ integration event messages and MassTransit setup extensions.

### Microservices Communication
- Asynchronous communication using **RabbitMQ**
- Uses RabbitMQ Publish/Subscribe Topic Exchange model
- Uses **MassTransit** as an abstraction over RabbitMQ
- Shared `BuildingBlocks.Messaging` library referenced across microservices

### YARP API Gateway (`YarpApiGateway`)
- Reverse proxy gateway built with **YARP** using the Gateway Routing pattern
- Configured with Routes, Clusters, Path Transforms, and Destinations for Identity, Account, and Notification services
- Rate limiting with `FixedWindowLimiter` (20 requests per 10 seconds on account routes)

### Client App (`EWallet.Client`)
- **ASP.NET Core Blazor WebAssembly** project
- Calls the **YARP API Gateway** using `HttpClient`
- Includes pages for Login, Register, My Account, Cards, Transactions (Deposit, Withdraw, Transfer), Address, Notifications, and Admin management (Users, Roles, Claims)

---

## Services & Ports

| Container | Description | Ports | URL / Info |
| :--- | :--- | :--- | :--- |
| **`ewalletclient`** | Blazor Client App | `6004:80` | [http://localhost:6004](http://localhost:6004) |
| **`yarpapi`** | YARP API Gateway | `6003:8080`, `6063:8081` | [http://localhost:6003](http://localhost:6003) |
| **`identityapi`** | Identity Microservice | `6000:8080`, `6060:8081` | [http://localhost:6000/swagger](http://localhost:6000/swagger) |
| **`accountapi`** | Account Microservice | `6001:8080`, `6061:8081` | [http://localhost:6001/swagger](http://localhost:6001/swagger) |
| **`notificationapi`** | Notification Microservice | `6002:8080`, `6062:8081` | [http://localhost:6002/swagger](http://localhost:6002/swagger) |
| **`identitydb`** | PostgreSQL (Identity) | `5433:5432` | DB: `IdentityDb` (`postgres` / `postgres`) |
| **`accountdb`** | PostgreSQL (Account) | `5434:5432` | DB: `AccountDb` (`postgres` / `postgres`) |
| **`notificationdb`** | MongoDB (Notification) | `27017:27017` | `mongodb://localhost:27017` |
| **`messagebroker`** | RabbitMQ Management | `5672:5672`, `15672:15672` | [http://localhost:15672](http://localhost:15672) (`guest` / `guest`) |
| **`mailhog`** | MailHog Email UI | `1025:1025`, `8025:8025` | [http://localhost:8025](http://localhost:8025) |

---

## How to Run the Project

You only need **Docker Desktop** to run the whole project, or you can install an IDE and the **.NET 8 SDK** to run it locally.

1. **Clone the repository**
   ```bash
   git clone https://github.com/ahmetsefaayaz/E-Wallet.git
   cd E-Wallet
   ```

2. **Start Docker Desktop**

3. **Run Docker Compose** from the `src/E-Wallet` folder:
   ```bash
   cd src/E-Wallet
   docker compose up --build -d
   ```

4. **Wait for Docker to create and start all containers**

5. **Open the Client App**:
   - Client UI: [http://localhost:6004](http://localhost:6004)
   - MailHog (Emails): [http://localhost:8025](http://localhost:8025)
   - RabbitMQ UI: [http://localhost:15672](http://localhost:15672)

### Default Admin Account

An admin user is created automatically when the Identity service starts:

- **Email**: `admin@mail.com`
- **Username**: `SuperUser`
- **Password**: `AdminPassword0`

---

## Project Structure

```text
E-Wallet/
├── diagrams/                              # Architecture diagrams
├── ports/                                 # Port mapping diagrams
├── postman_collection/                    # Postman collection for API testing
├── swagger_json/                          # Swagger JSON files
└── src/
    └── E-Wallet/
        ├── ApiGateways/
        │   └── YarpApiGateway/            # YARP Reverse Proxy Gateway
        ├── BuildingBlocks/
        │   ├── BuildingBlocks/            # Shared CQRS, Validation, Logging, Exceptions
        │   └── BuildingBlocks.Messaging/  # MassTransit & RabbitMQ Events
        ├── Services/
        │   ├── Identity/
        │   │   └── Identity.Api/          # Identity Microservice
        │   ├── Account/
        │   │   ├── Account.Domain/        # Domain Models, Value Objects, Domain Events
        │   │   ├── Account.Application/   # CQRS Commands, Queries, Event Handlers
        │   │   ├── Account.Infrastructure/# EF Core, PostgreSQL, Migrations
        │   │   └── Account.Api/           # Minimal API Endpoints
        │   └── Notification/
        │       └── Notification.Api/      # Notification Microservice
        ├── UI/
        │   └── EWallet.Client/            # Blazor WebAssembly Client App
        ├── compose.yaml                   # Docker Compose file
        └── E-Wallet.sln                   # Solution file
```
<p align="center">
<img width="1919" height="972" alt="image" src="https://github.com/user-attachments/assets/d82cf1cd-eb06-48a8-b836-61eb3738d115" />
</p>
<p align="center">
  <img width="1919" height="970" alt="E-Wallet Ports" src="https://github.com/user-attachments/assets/410c8a9d-bc52-4f08-ac09-05c4eca4632e" />
</p>

<p align="center">
  <img width="1919" height="968" alt="image" src="https://github.com/user-attachments/assets/85edc576-26d5-475e-91da-39b68b658c77" />
</p>
<p align="center">
  <img width="1919" height="971" alt="image" src="https://github.com/user-attachments/assets/337566d4-3d5a-494e-8933-c2f191997208" />
</p>
