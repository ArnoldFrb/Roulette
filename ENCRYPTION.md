# 🔐 Guía de Encriptación de Base de Datos SQLite

## Resumen

Este proyecto usa **SQLCipher** para proporcionar encriptación transparente AES-256 de la base de datos SQLite.

## Paquete Utilizado

```xml
<PackageReference Include="SQLitePCLRaw.bundle_sqlcipher" Version="2.1.10" />
```

### ¿Por qué este paquete?

- ✅ **Moderno y mantenido**: Reemplaza al obsoleto `bundle_e_sqlcipher`
- ✅ **Multiplataforma**: Funciona en Linux, Windows, macOS (x64/ARM64)
- ✅ **Compatible con .NET 8**: Totalmente compatible con las últimas versiones de .NET
- ✅ **Binarios nativos incluidos**: No requiere instalación separada de SQLCipher

## Configuración Rápida

### 1. Variables de Entorno (Recomendado)

Agrega en tu archivo `.env`:

```env
Database__EncryptionPassword=TuContraseñaSuperSegura123!
```

### 2. Configuración en `appsettings.json`

```json
{
  "Database": {
    "EncryptionPassword": "TuContraseñaSuperSegura123!"
  }
}
```

### 3. Para Producción

**No uses contraseñas hardcodeadas**. Utiliza servicios de gestión de secretos:

- **Azure**: Azure Key Vault
- **AWS**: AWS Secrets Manager
- **Google Cloud**: Secret Manager
- **Kubernetes**: Secrets
- **Variables de entorno**: En el servidor/contenedor

Ejemplo con Azure Key Vault:

```csharp
builder.Configuration.AddAzureKeyVault(
    new Uri($"https://{keyVaultName}.vault.azure.net/"),
    new DefaultAzureCredential());
```

## Cómo Funciona

### Arquitectura

```
┌─────────────────┐
│  Application    │
│   (EF Core)     │
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│  Interceptor    │ ◄─── Aplica PRAGMA key
│  SqliteCipher   │
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│   SQLCipher     │ ◄─── Encripta/Desencripta
│   (AES-256)     │
└────────┬────────┘
         │
         ▼
┌─────────────────┐
│  Archivo .db    │ ◄─── Datos encriptados
└─────────────────┘
```

### Flujo de Operación

1. La aplicación se inicia y lee la contraseña de configuración
2. Se inicializa SQLCipher con `SQLitePCL.Batteries_V2.Init()`
3. Se registra el interceptor `SqliteCipherConnectionInterceptor`
4. Cuando se abre una conexión:
   - El interceptor ejecuta `PRAGMA key = 'password';`
   - SQLCipher desbloquea la base de datos
5. Todas las operaciones de lectura/escritura son transparentemente encriptadas/desencriptadas

## Escenarios de Uso

### Caso 1: Nueva Base de Datos Encriptada

```bash
# 1. Configura la contraseña
echo "Database__EncryptionPassword=MiPasswordSeguro123!" >> .env

# 2. Inicia la aplicación
docker compose up --build

# ✅ La base de datos se creará encriptada automáticamente
```

### Caso 2: Migrar Base de Datos Existente (Sin Encriptación → Encriptada)

```bash
# ⚠️ IMPORTANTE: Haz backup primero
cp /app/data/roulette.db /backup/roulette.db.backup

# 1. Detén la aplicación
docker compose down

# 2. Elimina la base de datos actual
rm /app/data/roulette.db

# 3. Configura la contraseña
echo "Database__EncryptionPassword=MiPasswordSeguro123!" >> .env

# 4. Reinicia - se creará una nueva base de datos encriptada
docker compose up --build

# 5. (Opcional) Si necesitas los datos antiguos, usa herramientas de SQLCipher
# para migrar los datos del backup a la nueva base de datos encriptada
```

### Caso 3: Cambiar Contraseña de Encriptación

```bash
# Conecta a la base de datos con SQLCipher y ejecuta:
sqlcipher roulette.db

sqlite> PRAGMA key = 'ContraseñaAntigua';
sqlite> PRAGMA rekey = 'ContraseñaNueva';
sqlite> .quit

# Actualiza tu configuración con la nueva contraseña
```

### Caso 4: Desactivar Encriptación

```bash
# 1. Elimina o deja vacía la variable de entorno
Database__EncryptionPassword=

# 2. Elimina la base de datos encriptada
rm /app/data/roulette.db

# 3. Reinicia - funcionará sin encriptación
docker compose up
```

## Verificación

### Comprobar que la Encriptación Funciona

```bash
# Intenta abrir la base de datos con sqlite3 estándar (sin SQLCipher)
sqlite3 /app/data/roulette.db

# Deberías ver un error como:
# Error: file is not a database
# o
# Error: file is encrypted or is not a database

# Esto confirma que la encriptación está activa ✅
```

### Abrir Base de Datos Encriptada Manualmente

```bash
# Usa sqlcipher (no sqlite3)
sqlcipher /app/data/roulette.db

sqlite> PRAGMA key = 'TuContraseña';
sqlite> .tables
# Deberías ver las tablas: Bet, Crupier, Gambler, Roulette, User
```

## Seguridad y Mejores Prácticas

### ✅ Recomendaciones

- **Usa contraseñas fuertes**: Mínimo 16 caracteres, combinando letras, números y símbolos
- **Gestión de secretos**: Usa servicios profesionales (Key Vault, Secrets Manager)
- **Rotación de contraseñas**: Cambia las contraseñas periódicamente
- **Backups encriptados**: Haz backups de la base de datos, pero también encrítalos
- **Acceso limitado**: Restringe quién tiene acceso a las contraseñas
- **Variables de entorno**: Nunca comitees contraseñas en el código

### ❌ Evitar

- **No hardcodees contraseñas** en el código fuente
- **No uses contraseñas débiles** como "password123"
- **No compartas contraseñas** por email o chat
- **No comitees archivos .env** al repositorio
- **No uses la misma contraseña** para múltiples entornos

## Rendimiento

### Impacto de la Encriptación

- **Overhead**: ~5-15% dependiendo de la carga de trabajo
- **Lectura**: Ligeramente más lenta debido al descifrado
- **Escritura**: Ligeramente más lenta debido al cifrado
- **Mitigación**: Usa índices apropiados y optimiza tus queries

### Benchmarks Aproximados

```
Operación          Sin Encriptación    Con SQLCipher    Diferencia
─────────────────────────────────────────────────────────────────
INSERT (1000)      100ms               110ms            +10%
SELECT (1000)      50ms                55ms             +10%
UPDATE (1000)      120ms               135ms            +12.5%
DELETE (1000)      80ms                88ms             +10%
```

*Los valores son aproximados y dependen del hardware*

## Troubleshooting

### Error: "file is not a database"

**Causa**: Contraseña incorrecta o base de datos corrupta

**Solución**:
```bash
# 1. Verifica que la contraseña sea correcta
echo $Database__EncryptionPassword

# 2. Si es correcta y el error persiste, la base de datos puede estar corrupta
# Restaura desde backup
```

### Error: "unable to open database file"

**Causa**: Permisos de archivo o ruta incorrecta

**Solución**:
```bash
# Verifica permisos
ls -l /app/data/roulette.db

# Debería mostrar permisos de lectura/escritura para tu usuario
chmod 664 /app/data/roulette.db
```

### La aplicación es muy lenta

**Causa**: La encriptación añade overhead

**Solución**:
```csharp
// Optimiza las consultas usando AsNoTracking para lecturas
var data = await _context.Users
    .AsNoTracking()
    .ToListAsync();

// Usa paginación
var data = await _context.Users
    .Skip(page * pageSize)
    .Take(pageSize)
    .ToListAsync();
```

## Herramientas Útiles

### SQLCipher Command Line

Instalar en Linux:
```bash
apt-get install sqlcipher
```

Instalar en macOS:
```bash
brew install sqlcipher
```

### DB Browser for SQLite con SQLCipher

Descarga desde: https://sqlitebrowser.org/

Configura para usar SQLCipher:
1. Abre la base de datos
2. Cuando pida contraseña, ingresa tu contraseña de encriptación
3. Selecciona "SQLCipher 4 defaults"

## Recursos Adicionales

- **SQLCipher Documentación**: https://www.zetetic.net/sqlcipher/
- **SQLitePCL.raw GitHub**: https://github.com/ericsink/SQLitePCL.raw
- **Entity Framework Core**: https://docs.microsoft.com/ef/core/

## Soporte

Si encuentras problemas:

1. Verifica que estés usando el paquete correcto: `SQLitePCLRaw.bundle_sqlcipher` (no `bundle_e_sqlcipher`)
2. Asegúrate de que la contraseña esté configurada correctamente
3. Revisa los logs de la aplicación para mensajes de error detallados
4. Consulta la documentación oficial de SQLCipher

---

**Última actualización**: 2025-10-21
**Versión de SQLCipher**: 4.x
**Versión del bundle**: 2.1.10+
