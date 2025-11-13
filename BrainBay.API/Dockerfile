# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["BrainBay.API/BrainBay.API.csproj", "BrainBay.API/"]
COPY ["BrainBay.Application/BrainBay.Application.csproj", "BrainBay.Application/"]
COPY ["BrainBay.Core/BrainBay.Core.csproj", "BrainBay.Core/"]
COPY ["BrainBay.Infrastructure/BrainBay.Infrastructure.csproj", "BrainBay.Infrastructure/"]
RUN dotnet restore "BrainBay.API/BrainBay.API.csproj"

COPY . .
WORKDIR "/src/BrainBay.API"
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "BrainBay.API.dll"]
