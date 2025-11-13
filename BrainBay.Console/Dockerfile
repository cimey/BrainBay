# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app


FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["BrainBay.Console/BrainBay.Console.csproj", "BrainBay.Console/"]
COPY ["BrainBay.Application/BrainBay.Application.csproj", "BrainBay.Application/"]
COPY ["BrainBay.Core/BrainBay.Core.csproj", "BrainBay.Core/"]
COPY ["BrainBay.Infrastructure/BrainBay.Infrastructure.csproj", "BrainBay.Infrastructure/"]
RUN dotnet restore "BrainBay.Console/BrainBay.Console.csproj"

COPY . .
WORKDIR "/src/BrainBay.Console"
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "BrainBay.Console.dll"]
