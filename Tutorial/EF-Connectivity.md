# Conectar Entity Framework a una Base de Datos en vivo

## Resumen

Este manual te guiará en la implementación de Entity Framework Core con una base de datos PostgreSQL. Aunque el enfoque principal está en Supabase, también se incluyen alternativas como PostgreSQL local o en Docker.

## Estructura del Proyecto

El proyecto está organizado en las siguientes capas:
- **Discoteque.API**: Capa de presentación y controladores
- **Discoteque.Data**: Capa de acceso a datos con Entity Framework
- **Discoteque.Business**: Capa de lógica de negocio

## Configuración del Proyecto EF

### 1. Instalación de Dependencias

Ejecuta estos comandos en ambos proyectos (API y Data):

```bash
dotnet tool install --global dotnet-ef
cd Discoteque.API/
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL 
cd ../Discoteque.Data
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL 
```

### 2. Configuración de la Cadena de Conexión

#### Opción A: Usando Supabase

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

#### Opción B: Usando Docker (Recomendado para desarrollo local)

1. Asegúrate de tener Docker instalado
2. Usa el `docker-compose.yml` proporcionado:
```bash
docker-compose up -d
```

3. La cadena de conexión para Docker sería:
```json
{
  "ConnectionStrings": {
    "DiscotequeDatabase": "Host=localhost;Username=postgres;Password=postgres;Database=discoteque;Port=5432"
  }
}
```

### 3. Configuración del DbContext

El proyecto implementa el patrón Unit of Work y Repository. El `DiscotequeContext` ya está configurado:

```csharp
public class DiscotequeContext : DbContext
{
    public DiscotequeContext(DbContextOptions<DiscotequeContext> options) : base(options)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }

    // Tus DbSet aquí
    // public DbSet<TuEntidad> TuEntidad { get; set; }
}
```

### 4. Configuración en Program.cs

```csharp
builder.Services.AddDbContext<DiscotequeContext>(
    opt => {
        opt.UseNpgsql(builder.Configuration.GetConnectionString("DiscotequeDatabase"));
    }    
);
```

### 5. Variables de Entorno y Secrets

Para desarrollo local, usa user-secrets:

```bash
cd Discoteque.API
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DiscotequeDatabase" "tu-cadena-de-conexion"
```

## Patrón Repository y Unit of Work

El proyecto ya implementa estos patrones. Para usarlos:

```csharp
public class TuController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public TuController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> TuAccion()
    {
        var repositorio = _unitOfWork.GetRepository<TuEntidad>();
        // Usar el repositorio
        await _unitOfWork.SaveChangesAsync();
        return Ok();
    }
}
```

## Migraciones y Actualización de Base de Datos

1. Crea la migración inicial:

```bash
cd Discoteque.API
dotnet ef migrations add InitialCreate --project ../Discoteque.Data
```

2. Configura el entorno:

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

## Verificación de la Conexión

Para verificar que la conexión funciona, puedes agregar este código temporal en tu `Program.cs`:

```csharp
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<DiscotequeContext>();
    try
    {
        await context.Database.CanConnectAsync();
        Console.WriteLine("Conexión exitosa a la base de datos");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error de conexión: {ex.Message}");
    }
}
```

## Configuración de Supabase (Opcional)

Si decides usar Supabase:

1. **Acceso**: Inicia sesión en Supabase usando GitHub o SSO
2. **Creación del Proyecto**: 
   - Crea un nuevo proyecto
   - Configura nombre y contraseña
   - Selecciona el plan gratuito

3. **Credenciales**:
   Guarda de forma segura:
   - Claves API (públicas)
   - Rol de servicio (privado)
   - URL del proyecto
   - Secreto del token JWT

> 🔒 **Seguridad**: 
> - Nunca subas credenciales al control de versiones
> - Usa variables de entorno o user-secrets en desarrollo
> - En producción, usa servicios de configuración seguros

## Solución de Problemas Comunes

1. **Error de conexión**:
   - Verifica que el servidor esté accesible
   - Confirma las credenciales
   - Asegúrate que el firewall permite la conexión

2. **Problemas con migraciones**:
   - Elimina la carpeta Migrations si es necesario
   - Ejecuta `dotnet ef database drop --force`
   - Vuelve a crear las migraciones

3. **Errores de timestamp**:
   - El DbContext ya incluye la configuración necesaria para PostgreSQL
   - Si persisten problemas, verifica la zona horaria del servidor
