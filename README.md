# Roulette
 Prueba tecnica de reuleta

## Construir y Ejecutar
 docker compose up --build

## Solo Ejecutar
 docker compose up -d

## Logs
 docker compose logs -f

## Detener y eliminar contenedores (sin borrar volúmenes)
 docker compose down

## Detener y borrar también volúmenes
 docker compose down -v

## Migration
 dotnet ef migrations add InitialCreate (solo docker)
 dotnet ef database update (local)

 dotnet ef migrations add InitialCreate --project Roulette.Infrastructure.Data --startup-project Roulette.API (solo docker)
 dotnet ef database update --project Roulette.Infrastructure.Data --startup-project Roulette.API (local)
