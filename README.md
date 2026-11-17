<div align="center">

# E-Wallet Microservices

**An event-driven E-Wallet microservices project built with .NET 8, Blazor WebAssembly, YARP API Gateway, RabbitMQ, PostgreSQL, and MongoDB.**

[![.NET 8.0](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![Blazor](https://img.shields.io/badge/Blazor_WASM-512BD4?style=for-the-badge&logo=blazor&logoColor=white)](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL_17-4169E1?style=for-the-badge&logo=postgresql&logoColor=white)](https://www.postgresql.org/)
[![MongoDB](https://img.shields.io/badge/MongoDB-47A248?style=for-the-badge&logo=mongodb&logoColor=white)](https://www.mongodb.com/)
[![RabbitMQ](https://img.shields.io/badge/RabbitMQ-FF6600?style=for-the-badge&logo=rabbitmq&logoColor=white)](https://www.rabbitmq.com/)
[![Docker](https://img.shields.io/badge/Docker_Compose-2496ED?style=for-the-badge&logo=docker&logoColor=white)](https://www.docker.com/)
[![xUnit](https://img.shields.io/badge/xUnit-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://xunit.net/)
[![GitHub Actions](https://img.shields.io/badge/GitHub_Actions_CI-2088FF?style=for-the-badge&logo=githubactions&logoColor=white)](https://github.com/features/actions)

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
| **Reliability & Data Patterns** | Transactional Outbox Pattern, Idempotency Pattern, Optimistic Concurrency Control |
| **API & Gateway** | Carter, YARP Reverse Proxy, Rate Limiting (`FixedWindowLimiter`), Swagger / OpenAPI |
| **Libraries** | MediatR, FluentValidation, Mapster |
| **Messaging** | RabbitMQ, MassTransit (with EF Core Outbox & Message Retry) |
| **Databases** | PostgreSQL 17, Entity Framework Core 8, MongoDB |
| **Authentication** | ASP.NET Core Identity, JWT Bearer Authentication, Role, Claim & Ownership Authorization |
| **Email** | MailKit (SMTP), MailHog |
| **Testing & CI/CD** | xUnit, Moq, MockQueryable.Moq, FluentAssertions, GitHub Actions |
| **Containers** | Docker, Docker Compose, Nginx |

---

## Key Features & Patterns

- **Transactional Outbox Pattern**: Uses MassTransit's Entity Framework Core Outbox (`InboxState`, `OutboxMessage`, `OutboxState`) in both Identity and Account services so database changes and RabbitMQ events are saved atomically in the same transaction.
- **Idempotency**: Protects critical write operations (Create Account, Deposit, Withdraw, Transfer) from duplicate requests caused by network retries. Uses an `Idempotency-Key` header, MediatR `IdempotencyBehavior`, PostgreSQL `IdempotencyRecords` storage, and a daily `IdempotencyCleanupJob` background service.
- **Optimistic Concurrency Control**: Uses PostgreSQL's `xmin` row version token on wallet accounts to prevent race conditions and lost balance updates when concurrent transactions occur, returning `409 Conflict` on version mismatch.
- **Resource Ownership & Security**: Enforces user-level data isolation so users can only access or modify their own accounts, cards, transactions, addresses, and notifications, while admins retain full management access.

---

## What's in This Repository

### Identity Microservice (`Identity.Api`)
Handles user registration, login, roles, claims, and user addresses.
- ASP.NET Core Minimal APIs with .NET 8.0
- Uses **Carter** for Minimal API endpoints
- CQRS implementation using **MediatR**
- Validation and logging pipelines with **MediatR** and **FluentValidation**
- Authentication and authorization with **ASP.NET Core Identity** and **JWT tokens**
- Publishes `UserRegisteredIntegrationEvent` to RabbitMQ using the **MassTransit EF Core Outbox** when a user registers or when the default admin is seeded
- Global exception handling and automatic database seeding
- **PostgreSQL** database running in Docker

### Account Microservice (`Account.Api`, `Account.Application`, `Account.Domain`, `Account.Infrastructure`)
Manages user wallet accounts, cards, balances, and money transactions.
- Built with **DDD**, **CQRS**, and **Clean Architecture**
- Uses **Carter** for Minimal API endpoints
- CQRS commands and queries with **MediatR** and **FluentValidation**
- Domain models (`Account`, `Card`, `Transaction`, `IdempotencyRecord`), Value Objects, and Domain Events
- Supports creating/deleting accounts, managing cards, and running transactions (**Deposit**, **Withdraw**, and **Transfer**)
- **Optimistic Concurrency** on `Account` using PostgreSQL `xmin` row versioning
- **Idempotency** check via `IdempotencyService` and automatic cleanup via `IdempotencyCleanupJob` (`BackgroundService`)
- Dispatches domain events via `DomainEventDispatcher` and publishes `CardCreatedIntegrationEvent` and `TransactionCreatedIntegrationEvent` through the **MassTransit EF Core Outbox**
- Automatic entity auditing (`CreatedBy`, `CreatedAt`, `LastModifiedBy`, `LastModified`) via `AuditableInterceptor` and `ICurrentUserId`
- **PostgreSQL** database running in Docker with EF Core migrations and seed data

### Notification Microservice (`Notification.Api`)
Listens for events from other services, sends emails, and saves user notifications.
- ASP.NET Core Minimal APIs with .NET 8.0
- Uses **Carter** for Minimal API endpoints
- CQRS implementation with **MediatR** and **FluentValidation**
- Global exception handling and logging behaviors
- Consumes integration events (`UserRegisteredIntegrationEvent`, `CardCreatedIntegrationEvent`, `TransactionCreatedIntegrationEvent`) from **RabbitMQ** using **MassTransit**
- Sends emails through **MailKit** and **MailHog**
- **MongoDB** database running in Docker (with singleton `IMongoClient` registration)

### BuildingBlocks (`BuildingBlocks` & `BuildingBlocks.Messaging`)
Shared class libraries used by the microservices:
- **`BuildingBlocks`**: Base CQRS interfaces (`ICommand`, `IQuery`, `IIdempotentCommand`), MediatR pipeline behaviors (`ValidationBehavior`, `LoggingBehavior`, `IdempotencyBehavior`), custom exceptions (`BadRequestException`, `NotFoundException`, `ForbiddenAccessException`, `DuplicateRequestException`, `InternalServerException`), `GlobalExceptionHandler` (handling validation, concurrency, duplicate requests, and access errors), and pagination helpers.
- **`BuildingBlocks.Messaging`**: Shared RabbitMQ integration event messages and MassTransit setup extensions with **EF Core Outbox** and automatic **Message Retry** configuration.

### Microservices Communication
- Asynchronous communication using **RabbitMQ**
- Uses RabbitMQ Publish/Subscribe Topic Exchange model
- Uses **MassTransit** with **Entity Framework Outbox** and **Message Retry** policies
- Shared `BuildingBlocks.Messaging` library referenced across microservices

### YARP API Gateway (`YarpApiGateway`)
- Reverse proxy gateway built with **YARP** using the Gateway Routing pattern
- Configured with Routes, Clusters, Path Transforms, and Destinations for Identity, Account, and Notification services
- Rate limiting with `FixedWindowLimiter` (20 requests per 10 seconds on account routes)

### Client App (`EWallet.Client`)
- **ASP.NET Core Blazor WebAssembly** project
- Calls the **YARP API Gateway** over HTTPS using `HttpClient`
- Sends `Idempotency-Key` headers on account creation and transaction requests to prevent duplicate submissions
- Includes pages for Login, Register, My Account, Cards, Transactions (Deposit, Withdraw, Transfer), Address, Notifications, and Admin management (Accounts, Users, Roles, Claims)

### Unit Tests & CI Pipeline (`Tests/`)
Comprehensive unit test suite built with **xUnit**, **Moq**, **MockQueryable.Moq**, and **FluentAssertions**, automatically executed on every push and pull request to `main` via **GitHub Actions** (`.github/workflows/ci.yml`):
- **`Account.UnitTests`**:
  - **Domain Tests**: Tests `Account` aggregate business rules (deposits, withdrawals, incoming/outgoing transfers, insufficient balance checks, maximum 3 cards limit, card updates/removals, and domain event raising) as well as DDD Value Objects (`CardNumber`, `CardExpiryDate`, `CardHolderName`, `CardSecurityNumber`, value equality, and validation).
  - **Application Tests**: Tests CQRS Command & Query Handlers (`CreateCardCommandHandler`, `DepositTransactionCommandHandler`, `WithdrawTransactionCommandHandler`, `TransferMoneyCommandHandler`, `GetAccountByIdQueryHandler`), FluentValidation rules (`OutgoingTransactionValidator`), and Domain Event Handlers (`CardCreatedEventHandler`, `TransactionCreatedEventHandler`).
- **`Identity.UnitTests`**:
  - Tests authentication and user management handlers (`LoginCommandHandler`, `RegisterCommandHandler`) and input validators (`LoginValidator`) using mocked `UserManager<User>` and `IJwtCreator`.
- **`Notification.UnitTests`**:
  - Tests notification command handlers (`MarkAsReadCommandHandler`) and MassTransit RabbitMQ consumers (`TransactionCreatedIntegrationEventHandler`) verifying email dispatch (`IEmailService`) and MongoDB persistence (`INotificationRepository`).

To run all unit tests locally:
```bash
dotnet test src/E-Wallet/E-Wallet.sln
```

---

## Services & Ports

| Container | Description | Ports | URL / Info |
| :--- | :--- | :--- | :--- |
| **`ewalletclient`** | Blazor Client App | `6004:80` | [http://localhost:6004](http://localhost:6004) |
| **`yarpapi`** | YARP API Gateway | `6003:8080`, `6063:8081` | [https://localhost:6063](https://localhost:6063) |
| **`identityapi`** | Identity Microservice | `6000:8080`, `6060:8081` | [http://localhost:6000/swagger](http://localhost:6000/swagger) |
| **`accountapi`** | Account Microservice | `6001:8080`, `6061:8081` | [http://localhost:6001/swagger](http://localhost:6001/swagger) |
| **`notificationapi`** | Notification Microservice | `6002:8080`, `6062:8081` | [http://localhost:6002/swagger](http://localhost:6002/swagger) |
| **`identitydb`** | PostgreSQL (Identity) | `5433:5432` | DB: `IdentityDb` |
| **`accountdb`** | PostgreSQL (Account) | `5434:5432` | DB: `AccountDb` |
| **`notificationdb`** | MongoDB (Notification) | `27017:27017` | `mongodb://localhost:27017` |
| **`messagebroker`** | RabbitMQ Management | `5672:5672`, `15672:15672` | [http://localhost:15672](http://localhost:15672) |
| **`mailhog`** | MailHog Email UI | `1025:1025`, `8025:8025` | [http://localhost:8025](http://localhost:8025) |

---

## How to Run the Project

You only need **Docker Desktop** to run the whole project, or you can install an IDE and the **.NET 8 SDK** to run it locally.

1. **Clone the repository**
   ```bash
   git clone https://github.com/ahmetsefaayaz/E-Wallet.git
   cd E-Wallet
   ```

2. **Create a `.env` file** inside the `src/E-Wallet` folder with your environment variables:
   ```env
   IDENTITY_DB_USER=postgres
   IDENTITY_DB_PASSWORD=postgres
   ACCOUNT_DB_USER=postgres
   ACCOUNT_DB_PASSWORD=postgres
   RABBITMQ_USER=guest
   RABBITMQ_PASSWORD=guest
   JWT_SECRET=YourSuperSecretJwtKeyWithAtLeast32BytesLength!
   KESTREL_PASSWORD=YourCertPassword
   ```

3. **Start Docker Desktop**

4. **Run Docker Compose** from the `src/E-Wallet` folder:
   ```bash
   cd src/E-Wallet
   docker compose up --build -d
   ```

5. **Wait for Docker to create and start all containers**

6. **Open the Client App**:
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
├── .github/
│   └── workflows/
│       └── ci.yml                         # GitHub Actions CI pipeline (build & test)
├── diagrams/                              # Architecture diagrams
├── ports/                                 # Port mapping diagrams
├── postman_collection/                    # Postman collection for API testing
├── swagger_json/                          # Swagger JSON files
└── src/
    └── E-Wallet/
        ├── ApiGateways/
        │   └── YarpApiGateway/            # YARP Reverse Proxy Gateway
        ├── BuildingBlocks/
        │   ├── BuildingBlocks/            # Shared CQRS, Validation, Logging, Idempotency, Exceptions
        │   └── BuildingBlocks.Messaging/  # MassTransit, RabbitMQ & Outbox Extensions
        ├── Services/
        │   ├── Identity/
        │   │   └── Identity.Api/          # Identity Microservice
        │   ├── Account/
        │   │   ├── Account.Domain/        # Domain Models, Value Objects, Domain Events
        │   │   ├── Account.Application/   # CQRS Commands, Queries, Idempotency, Event Handlers
        │   │   ├── Account.Infrastructure/# EF Core, PostgreSQL, Outbox, Background Jobs, Migrations
        │   │   └── Account.Api/           # Minimal API Endpoints
        │   └── Notification/
        │       └── Notification.Api/      # Notification Microservice
        ├── Tests/
        │   ├── Account.UnitTests/         # Domain & Application unit tests for Account service
        │   ├── Identity.UnitTests/        # Command & Validator unit tests for Identity service
        │   └── Notification.UnitTests/    # Command & EventHandler unit tests for Notification service
        ├── UI/
        │   └── EWallet.Client/            # Blazor WebAssembly Client App
        ├── compose.yaml                   # Docker Compose file
        └── E-Wallet.sln                   # Solution file
```

---

## Screenshots

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
