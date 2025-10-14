# Etapa base (runtime)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Roulette.API/Roulette.API.csproj", "Roulette.API/"]
COPY ["Roulette.Application/Roulette.Application.csproj", "Roulette.Application/"]
COPY ["Roulette.Domain/Roulette.Domain.csproj", "Roulette.Domain/"]
COPY ["Roulette.Infrastructure.Data/Roulette.Infrastructure.Data.csproj", "Roulette.Infrastructure.Data/"]
COPY ["Roulette.Infrastructure.Redis/Roulette.Infrastructure.Redis.csproj", "Roulette.Infrastructure.Redis/"]
COPY ["Roulette.Infrastructure.Security/Roulette.Infrastructure.Security.csproj", "Roulette.Infrastructure.Security/"]
RUN dotnet restore "./Roulette.API/Roulette.API.csproj"
COPY . .
WORKDIR "/src/Roulette.API"
RUN dotnet build "./Roulette.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publicación
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Roulette.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Imagen final
FROM base AS final
WORKDIR /app

# Crear usuario no root y carpetas necesarias con permisos
RUN adduser --disabled-password --home /app rouletteuser \
    && mkdir -p /app/data /app/DataProtection-Keys \
    && chown -R rouletteuser /app

# Crear directorio de logs y asignar permisos
RUN mkdir -p /app/logs && chown -R rouletteuser /app/logs
VOLUME ["/app/logs"]

# Cambiar a usuario seguro
USER rouletteuser

# Copiar la publicación
COPY --from=publish /app/publish .

# Establecer variable de entorno para DataProtection
ENV ASPNETCORE_DATA_PROTECTION_KEYS=/app/DataProtection-Keys

# Volumen persistente para SQLite y DataProtection
VOLUME ["/app/data", "/app/DataProtection-Keys"]

ENTRYPOINT ["dotnet", "Roulette.API.dll"]