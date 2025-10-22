# 🎰 Roulette
Prueba técnica de ruleta desarrollada en **.NET 8**, con persistencia en **SQLite**, cache con **Redis**, autenticación **JWT**, logging con **Serilog** y soporte para **Docker Compose**.

## 📁 Estructura del repositorio (resumen) 
```bash
/Roulette
    ├── Roulette.API
        ├── Dockerfile 
    ├── Roulette.Application
    ├── Roulette.Domain
    ├── Roulette.Infrastructure.Data
    ├── Roulette.Infrastructure.Redis
    ├── Roulette.Infrastructure.Security
    ├── docker-compose.yml
    ├── .env
    └── README.md
```

---

## 🔧 Requisitos
- .NET 8 SDK 
- Docker & Docker Compose (para ejecutar en contenedores)
- dotnet-ef (opcional, para manejar migraciones)
```bash
  dotnet tool install --global dotnet-ef
```

## 🏗️ Construir y Ejecutar
```bash
docker compose up --build
```

## 🏗️ Construir y Limpiar cache
```bash
docker compose build -no--cache
```

## 🚀 Solo Ejecutar
```bash
docker compose up -d
```

## 📜 Logs
```bash
docker compose logs -f
```

## 🧹 Detener y eliminar contenedores (sin borrar volúmenes)
```bash
docker compose down
```

## 💣 Detener y borrar también volúmenes
```bash
docker compose down -v
```

## 🧩 Migraciones
> ⚙️ Ejecuta estos comandos dentro del contenedor o en tu entorno local.

### Crear migración (local y Docker)
```bash
dotnet ef migrations add InitialCreate
```

### Aplicar migraciones (local)
```bash
dotnet ef database update
```

### Crear migración adicional (Local y Docker)
```bash
dotnet ef migrations add InitialCreate --project Roulette.Infrastructure.Data --startup-project Roulette.API
```

### Aplicar migración adicional (local)
```bash
dotnet ef database update --project Roulette.Infrastructure.Data --startup-project Roulette.API
```

---

## 📦 Variables de entorno (.env)
Crea el archivo `.env`

### Ejemplo:
```env
ASPNETCORE_ENVIRONMENT=Development
Database__EncryptionPassword=MiClaveEncrypion_Super_Secreta_1234
ConnectionStrings__DefaultConnection=Data Source=/app/data/roulette.db
Redis__ConnectionString=redis:6379
Redis__InstanceName=roulette:
Jwt__Key=MiClaveJWT_Super_Secreta_1234
Jwt__Issuer=Roulette.API
Jwt__Audience=Roulette.Client
Jwt__ExpirationMinutes=60
```

---

## 🌐 Acceso rápido
- **API:** [http://localhost:8080](http://localhost:8080)
- **Swagger:** [http://localhost:8080/swagger](http://localhost:8080/swagger)

---

## 👤 Datos de pruebas

| Rol | Usuario | Contraseña |
|-----|----------|-------------|
| Crupier | crupier1 | password1 |
| Gambler | user0 | (sin login, solo apuestas) |
| Gambler | user1 | (sin login, solo apuestas) |
| Gambler | user2 | (sin login, solo apuestas) |
| Gambler | user3 | (sin login, solo apuestas) |

---

## 🧠 Notas

- 🐳 Si usas Docker, las migraciones se aplican automáticamente al iniciar.
- 🔐 Usa el endpoint `/api/auth` para autenticar al crupier y obtener el token JWT.
- 🪵 Los logs persistentes, se guarda en `/app/logs/roulette-.log`.
- 🔒 La base de datos SQLite ahora soporta encriptación AES-256 con SQLCipher.
