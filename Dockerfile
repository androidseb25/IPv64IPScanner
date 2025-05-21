# Base Image für die Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Build Image
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG TARGETARCH
WORKDIR /src

# Projektdateien kopieren und Abhängigkeiten wiederherstellen
COPY ["IPv64IPScanner.csproj", "."]
RUN dotnet restore "./IPv64IPScanner.csproj" --arch $TARGETARCH

# Quellcode kopieren und Projekt kompilieren
COPY . .
RUN dotnet build "./IPv64IPScanner.csproj" -c Release --arch $TARGETARCH -o /app/build

# Publish Image
FROM build AS publish
RUN dotnet publish "./IPv64IPScanner.csproj" -c Release --arch $TARGETARCH -o /app/publish

# Final Image für die Runtime
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IPv64IPScanner.dll"]
