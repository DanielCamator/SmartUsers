FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/backend/AIService/Api/Api.csproj", "src/backend/AIService/Api/"]
COPY ["src/backend/AIService/Application/Application.csproj", "src/backend/AIService/Application/"]
COPY ["src/backend/AIService/Domain/Domain.csproj", "src/backend/AIService/Domain/"]
COPY ["src/backend/AIService/Infrastructure/Infrastructure.csproj", "src/backend/AIService/Infrastructure/"]
RUN dotnet restore "./src/backend/AIService/Api/Api.csproj"
COPY . .
WORKDIR "/src/backend/AIService/Api"
RUN dotnet build "./Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Api.dll"]