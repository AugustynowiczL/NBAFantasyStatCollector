# Base image for running the app (runtime environment)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Install SQLite client in the container (for Linux-based containers)
RUN apt-get update && apt-get install -y sqlite3 libsqlite3-dev

# Build image for restoring dependencies and building the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["NBAFantasy.csproj", "."]
RUN dotnet restore "./NBAFantasy.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "./NBAFantasy.csproj" -c Release -o /app/build

# Publish image for optimizing the app for production
FROM build AS publish
RUN dotnet publish "./NBAFantasy.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final image to run the app and apply migrations
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Add the step to run EF Core migrations
ENTRYPOINT ["dotnet", "NBAFantasy.dll"]
