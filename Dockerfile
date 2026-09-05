# Base image
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base

# Install curl for Coolify health checks
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

USER $APP_UID

WORKDIR /app

EXPOSE 8080
EXPOSE 8081


# Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

ARG BUILD_CONFIGURATION=Release

WORKDIR /src

COPY ["Portfolio.Api.csproj", "."]

RUN dotnet restore "./Portfolio.Api.csproj"

COPY . .

WORKDIR "/src/."

RUN dotnet build "./Portfolio.Api.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/build


# Publish
FROM build AS publish

ARG BUILD_CONFIGURATION=Release

RUN dotnet publish "./Portfolio.Api.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false


# Production
FROM base AS final

WORKDIR /app

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Portfolio.Api.dll"]
