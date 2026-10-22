FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Services/Account/Account.Api/Account.Api.csproj", "Services/Account/Account.Api/"]
COPY ["Services/Account/Account.Infrastructure/Account.Infrastructure.csproj", "Services/Account/Account.Infrastructure/"]
COPY ["Services/Account/Account.Application/Account.Application.csproj", "Services/Account/Account.Application/"]
COPY ["Services/Account/Account.Domain/Account.Domain.csproj", "Services/Account/Account.Domain/"]
COPY ["BuildingBlocks/BuildingBlocks/BuildingBlocks.csproj", "BuildingBlocks/BuildingBlocks/"]
RUN dotnet restore "Services/Account/Account.Api/Account.Api.csproj"
COPY . .
WORKDIR "/src/Services/Account/Account.Api"
RUN dotnet build "./Account.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Account.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Account.Api.dll"]
