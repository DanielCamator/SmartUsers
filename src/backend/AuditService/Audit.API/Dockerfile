FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["src/backend/AuditService/Audit.API/Audit.API.csproj", "src/backend/AuditService/Audit.API/"]
COPY ["src/backend/AuditService/Audit.Application/Audit.Application.csproj", "src/backend/AuditService/Audit.Application/"]
COPY ["src/backend/AuditService/Audit.Domain/Audit.Domain.csproj", "src/backend/AuditService/Audit.Domain/"]
COPY ["src/backend/AuditService/Audit.Infrastructure/Audit.Infrastructure.csproj", "src/backend/AuditService/Audit.Infrastructure/"]
COPY ["src/backend/Shared/Shared.Contracts/Shared.Contracts.csproj", "src/backend/Shared/Shared.Contracts/"]

RUN dotnet restore "src/backend/AuditService/Audit.API/Audit.API.csproj"
COPY . .
WORKDIR "/src/src/backend/AuditService/Audit.API"
RUN dotnet build "Audit.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Audit.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Audit.API.dll"]