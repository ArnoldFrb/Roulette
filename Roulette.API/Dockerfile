# Consulte https://aka.ms/customizecontainer para aprender a personalizar su contenedor de depuración y cómo Visual Studio usa este Dockerfile para compilar sus imágenes para una depuración más rápida.

# Esta fase se usa cuando se ejecuta desde VS en modo rápido (valor predeterminado para la configuración de depuración)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# Esta fase se usa para compilar el proyecto de servicio
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Roulette.API/Roulette.API.csproj", "Roulette.API/"]
COPY ["Roulette.Application/Roulette.Application.csproj", "Roulette.Application/"]
COPY ["Roulette.Domain/Roulette.Domain.csproj", "Roulette.Domain/"]
COPY ["Roulette.Infrastructure/Roulette.Infrastructure.csproj", "Roulette.Infrastructure/"]
RUN dotnet restore "./Roulette.API/Roulette.API.csproj"
COPY . .
WORKDIR "/src/Roulette.API"
RUN dotnet build "./Roulette.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Esta fase se usa para publicar el proyecto de servicio que se copiará en la fase final.
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Roulette.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Esta fase se usa en producción o cuando se ejecuta desde VS en modo normal (valor predeterminado cuando no se usa la configuración de depuración)
FROM base AS final
WORKDIR /app

# --- Crear usuario no root aquí ---
RUN adduser --disabled-password --home /app rouletteuser

# Crear directorio para la base de datos SQLite y asignar permisos
RUN mkdir -p /app/data && chown -R rouletteuser /app

# Copiar archivos publicados
COPY --from=publish /app/publish .

# Cambiar al usuario seguro
USER rouletteuser

VOLUME ["/app/data"]

ENTRYPOINT ["dotnet", "Roulette.API.dll"]