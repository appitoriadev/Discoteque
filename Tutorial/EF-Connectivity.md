# Conectar Entity Framework a una Base de Datos en vivo en Supabase

## Resumen

Este manual te guiará en la implementación de Entity Framework Core con una base de datos PostgreSQL alojada en Supabase. Aunque el enfoque está en PostgreSQL, los conceptos son aplicables a SQL Server u otros motores de base de datos relacionales.

## Configuración del Proyecto EF

La integración con Supabase requiere dos componentes principales:

1. **Configuración de la conexión**: Aunque Supabase ofrece un SDK completo con múltiples funcionalidades, en este proyecto utilizaremos Entity Framework Core directamente para demostrar el patrón Unit of Work y tener mayor control sobre nuestras operaciones de datos.

2. **Instalación de dependencias**: Necesitamos el proveedor de PostgreSQL para Entity Framework Core. Ejecuta estos comandos en ambos proyectos (API y Data):

```bash
dotnet tool install --global dotnet-ef
cd Discoteque.API/
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL 
cd ../Discoteque.Data
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL 
```

### Configuración de la Cadena de Conexión

Agrega la siguiente configuración en `appsettings.Development.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }    
  },
  "ConnectionStrings": {
    "DiscotequeDatabase": "Host=[MYSERVER];Username=[MYUSERNAME];Password=[MYPASSWORD];Database=[MYDATABASE]"
  }
}
```

> ⚠️ **Importante**: Por seguridad, nunca subas credenciales al control de versiones. Utiliza variables de entorno o servicios de configuración seguros en producción.

### Configuración del DbContext

Actualiza el `Program.cs` para utilizar PostgreSQL:

```csharp
builder.Services.AddDbContext<DiscotequeContext>(
    opt => {
        opt.UseNpgsql(builder.Configuration.GetConnectionString("DiscotequeDatabase"));
    }    
);
```

### Parámetros de Conexión Supabase

Los valores necesarios para la cadena de conexión son:
- **Host**: URL del servidor Supabase
- **Username**: Normalmente "postgres"
- **Puerto**: 5432 por defecto
- **Database**: Nombre de tu base de datos en Supabase

### Migraciones y Actualización de Base de Datos

1. Crea la migración inicial desde el directorio del proyecto API:

```bash
dotnet ef migrations add InitialCreate --project ../Discoteque.Data
```

2. Configura el entorno de desarrollo:

Para Windows (PowerShell):
```bash
$Env:ASPNETCORE_ENVIRONMENT = "Development"
```

Para Mac/Linux:
```bash
export ASPNETCORE_ENVIRONMENT=Development
```

3. Aplica la migración:
```bash
dotnet ef database update
```

### Configuración Adicional del DbContext

Para evitar problemas con el manejo de fechas en PostgreSQL, actualiza el constructor del DbContext:

```csharp
public DiscotequeContext(
        DbContextOptions<DiscotequeContext> options) 
        : base(options)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }
```

## Configuración de Supabase

Supabase es una plataforma que ofrece una base de datos PostgreSQL gestionada, con características adicionales como autenticación y APIs automáticas. Su nivel gratuito es ideal para desarrollo y aprendizaje.

### Proceso de Configuración

1. **Acceso**: Inicia sesión en Supabase usando GitHub o SSO (Google u otros proveedores).

2. **Creación del Proyecto**: 
   - Desde el dashboard principal, selecciona "Crear nuevo proyecto"
   - Configura el nombre y la contraseña de la base de datos
   - Selecciona el plan gratuito para desarrollo

3. **Credenciales del Proyecto**:
   Después de la creación, guardarás de forma segura:
   - Claves API (públicas)
   - Rol de servicio (privado)
   - URL del proyecto
   - Secreto del token JWT

> 🔒 **Seguridad**: Mantén las credenciales de servicio y tokens JWT en un lugar seguro y nunca las expongas en el código fuente.
