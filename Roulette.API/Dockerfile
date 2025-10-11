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

# Crear usuario no root
RUN adduser --disabled-password --home /app rouletteuser

# Crear directorio de datos y dar permisos
RUN mkdir -p /app/data && chown -R rouletteuser /app

# Copiar la publicación
COPY --from=publish /app/publish .

# Cambiar a usuario seguro
USER rouletteuser

# Volumen persistente para SQLite
VOLUME ["/app/data"]

ENTRYPOINT ["dotnet", "Roulette.API.dll"]
