FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["src/backend/UserService/User.API/User.API.csproj", "src/backend/UserService/User.API/"]
COPY ["src/backend/UserService/User.Application/User.Application.csproj", "src/backend/UserService/User.Application/"]
COPY ["src/backend/UserService/User.Domain/User.Domain.csproj", "src/backend/UserService/User.Domain/"]
COPY ["src/backend/UserService/User.Infrastructure/User.Infrastructure.csproj", "src/backend/UserService/User.Infrastructure/"]
COPY ["src/backend/Shared/Shared.Contracts/Shared.Contracts.csproj", "src/backend/Shared/Shared.Contracts/"]

RUN dotnet restore "src/backend/UserService/User.API/User.API.csproj"
COPY . .
WORKDIR "/src/src/backend/UserService/User.API"
RUN dotnet build "User.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "User.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "User.API.dll"]