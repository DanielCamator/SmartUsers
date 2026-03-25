FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["src/backend/AuthService/Auth.API/Auth.API.csproj", "src/backend/AuthService/Auth.API/"]
COPY ["src/backend/AuthService/Auth.Application/Auth.Application.csproj", "src/backend/AuthService/Auth.Application/"]
COPY ["src/backend/AuthService/Auth.Domain/Auth.Domain.csproj", "src/backend/AuthService/Auth.Domain/"]
COPY ["src/backend/AuthService/Auth.Infrastructure/Auth.Infrastructure.csproj", "src/backend/AuthService/Auth.Infrastructure/"]
COPY ["src/backend/Shared/Shared.Contracts/Shared.Contracts.csproj", "src/backend/Shared/Shared.Contracts/"]

RUN dotnet restore "src/backend/AuthService/Auth.API/Auth.API.csproj"
COPY . .
WORKDIR "/src/src/backend/AuthService/Auth.API"
RUN dotnet build "Auth.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Auth.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Auth.API.dll"]