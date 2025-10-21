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
ConnectionStrings__DefaultConnection=Data Source=/app/data/roulette.db
Database__EncryptionPassword=TuContraseñaSuperSegura123!
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

## 🔐 Encriptación de la Base de Datos

La aplicación soporta **encriptación de la base de datos SQLite** usando **SQLCipher** (versión moderna mantenida), que proporciona encriptación transparente AES-256.

📖 **[Ver guía completa de encriptación](ENCRYPTION.md)** para información detallada, troubleshooting y mejores prácticas.

### ¿Cómo funciona?

1. **SQLCipher** reemplaza el módulo estándar de SQLite con una versión que encripta toda la base de datos
2. La encriptación se aplica automáticamente cuando se proporciona una contraseña en la configuración
3. Los datos se cifran/descifran transparentemente en cada operación de lectura/escritura

### Configuración

#### Opción 1: Archivo de configuración
Agrega la contraseña en `appsettings.json` o `appsettings.Development.json`:

```json
{
  "Database": {
    "EncryptionPassword": "TuContraseñaSuperSegura123!"
  }
}
```

#### Opción 2: Variable de entorno (Recomendado para producción)
Agrega en tu archivo `.env`:

```env
Database__EncryptionPassword=TuContraseñaSuperSegura123!
```

### ⚠️ Importante

- **No pierdas la contraseña**: Sin ella, no podrás acceder a los datos
- **Primera vez**: Si encriptas una base de datos existente, necesitarás recrearla (hacer backup primero)
- **Migrar base de datos existente**: Si tienes una base de datos sin encriptar y quieres encriptarla:
  ```bash
  # 1. Backup de la base de datos actual
  cp roulette.db roulette.db.backup
  
  # 2. Elimina la base de datos actual
  rm roulette.db
  
  # 3. Configura la contraseña y reinicia la aplicación
  # La base de datos se creará encriptada automáticamente
  ```
- **Producción**: Usa variables de entorno o secretos seguros (Azure Key Vault, AWS Secrets Manager, etc.)
- **Sin contraseña**: Si no se proporciona contraseña, la base de datos funcionará sin encriptación

### Verificar que funciona

Después de configurar la encriptación, la base de datos estará protegida. Si intentas abrirla con un visor SQLite estándar sin la contraseña, obtendrás un error de "base de datos corrupta" o "no es una base de datos SQLite".

### Tecnología utilizada

- **Paquete**: `SQLitePCLRaw.bundle_sqlcipher` (versión 2.1.10+)
  - Reemplaza al obsoleto `bundle_e_sqlcipher`
  - Activamente mantenido y compatible con .NET 8
  - Incluye binarios nativos de SQLCipher para múltiples plataformas
- **Algoritmo**: AES-256 en modo CBC
- **Compatible con**: Linux, Windows, macOS (x64/ARM64)

---

## 🧠 Notas

- 🐳 Si usas Docker, las migraciones se aplican automáticamente al iniciar.
- 🔐 Usa el endpoint `/api/auth` para autenticar al crupier y obtener el token JWT.
- 🪵 Los logs persistentes, se guarda en `/app/logs/roulette-.log`.
- 🔒 La base de datos SQLite ahora soporta encriptación AES-256 con SQLCipher.
