# Dotnet: The Discoteque

## Creación de una API Web con .NET

Este tutorial te guiará en la creación de una API Web robusta utilizando .NET. Para información detallada, consulta la documentación oficial:

[Tutorial: Create a controller-based web API with ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-9.0&tabs=visual-studio)

### Estructura del Proyecto

Crearemos una solución con arquitectura en capas, siguiendo los principios de Clean Architecture. Primero, prepara tu entorno de desarrollo:

```bash
# Para usuarios de Windows
cd C:/
# Para usuarios de Linux/MacOS, omite el paso anterior

mkdir code && cd code
mkdir Discoteque && cd Discoteque
```

### Creación de la Solución

Implementaremos una arquitectura en capas con los siguientes componentes:

- **API**: Capa de presentación y endpoints
- **Business**: Lógica de negocio y servicios
- **Data**: Acceso a datos y modelos

```bash
# Crear la estructura base
dotnet new sln -n Discoteque
dotnet new classlib -o Discoteque.Business
dotnet new webapi -o Discoteque.API
dotnet new classlib -o Discoteque.Data

# Agregar proyectos a la solución
dotnet sln add Discoteque.API/
dotnet sln add Discoteque.Business/
dotnet sln add Discoteque.Data/
```

### Verificación de la Estructura

Confirma que la estructura se creó correctamente:

```bash
ls -a
# Estructura esperada:
# .                       Discoteque.API          Discoteque.sln
# ..                      Discoteque.Business     Discoteque.Data
```

Compila la solución para verificar la integridad:

```bash
dotnet build
```

### Configuración de Dependencias

Instala los paquetes necesarios para Entity Framework Core:

```bash
cd Discoteque.API/
dotnet add package Microsoft.EntityFrameworkCore.InMemory
cd ../Discoteque.Data/
dotnet add package Microsoft.EntityFrameworkCore.InMemory
cd ..
```

### Configuración del Entorno de Desarrollo

VS Code requiere archivos de configuración específicos para depuración y ejecución. Crea los siguientes archivos en la carpeta `.vscode`:

1. **launch.json**: Configura los perfiles de depuración
2. **tasks.json**: Define las tareas de compilación y ejecución
3. **settings.json**: Establece la configuración del proyecto

Los archivos de configuración completos se mantienen sin cambios, ya que son esenciales para la correcta configuración del entorno.

## Implementación del Dominio

### Modelos de Datos

Crea la estructura de modelos en `Discoteque.Data/Models/`:

1. **BaseEntity.cs**: Clase base para todos los modelos

```csharp
namespace Discoteque.Data.Models;

public class BaseEntity<TId> where TId : struct
{
    public TId Id { get; set; }
}
```

2. **Artist.cs**: Modelo para artistas musicales

```csharp
namespace Discoteque.Data.Models;

public class Artist: BaseEntity<int>
{
    public string Name { get; set; }
    public string Label { get; set; }
    public bool IsOnTour { get; set; }
}
```

3. **Album.cs**: Modelo para álbumes con géneros musicales

```csharp
namespace Discoteque.Data.Models;

public class Album: BaseEntity<int>
{
    public string Name { get; set; }
    public int Year { get; set; }
    public Genres Genre { get; set; } = Genres.Unknown;
}

public enum Genres
{
    Rock,
    Metal,
    Salsa,
    Merengue,
    Urban,
    Folk,
    Indie,
    Techno,
    Unknown
}
```

### Contexto de Base de Datos

El `DiscotequeContext` gestiona la conexión con la base de datos:

```csharp
namespace Discoteque.Data;

public class DiscotequeContext : DbContext
{
    public DiscotequeContext(DbContextOptions<DiscotequeContext> options)
        : base(options)
    {
    }

    public DbSet<Artist> Artists { get; set; } = null!;
    public DbSet<Album> Albums { get; set; } = null!;
}
```

### Referencias entre Proyectos

Establece las dependencias entre proyectos:

```bash
dotnet add Discoteque.API reference Discoteque.Data/Discoteque.Data.csproj
dotnet add Discoteque.API reference Discoteque.Business/Discoteque.Business.csproj
dotnet add Discoteque.Business/ reference Discoteque.Data/Discoteque.Data.csproj
```

## Implementación de la API

### Configuración Inicial

Registra los servicios necesarios en `Program.cs`:

```csharp
builder.Services.AddDbContext<DiscotequeContext>(
    opt => opt.UseInMemoryDatabase("Discoteque")
);
```

### Paquetes Necesarios

Instala las herramientas y paquetes para el desarrollo de la API:

```bash
cd Discoteque.API
dotnet add package Microsoft.AspNetCore.OpenApi -v 9.0.2
dotnet add package Microsoft.EntityFrameworkCore.Design -v 9.0.2
dotnet add package Microsoft.EntityFrameworkCore.InMemory -v 9.0.2
dotnet add package Microsoft.EntityFrameworkCore.SqlServer -v 9.0.2
dotnet add package Microsoft.VisualStudio.Web.CodeGeneration.Design -v 9.0.0
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL -v 9.0.3
dotnet add package Swashbuckle.AspNetCore -v 7.3.1

# Herramientas de desarrollo
dotnet tool install -g dotnet-aspnet-codegenerator
dotnet tool update -g dotnet-ef
dotnet add package Microsoft.EntityFrameworkCore.Tools -v 9.0.2
```

## Capa de Servicios

### Definición de Interfaces

Crea la interfaz base para el servicio de artistas:

```csharp
namespace Discoteque.Business.IServices
{
    public interface IArtistsService
    {
        Task<IEnumerable<Artist>> GetArtistsAsync();
        Task<Artist> GetById(int id);
        Task<Artist> CreateArtist(Artist artist);
        Task<Artist> UpdateArtist(Artist artist);
    }
}
```

### Implementación del Servicio

La implementación inicial del servicio:

```csharp
namespace Discoteque.Business.Services;

public class ArtistsService : IArtistsService
{
    private IUnitOfWork _unitOfWork;

    public ArtistsService(IUnitOfWork unitofWork)
    {
        _unitOfWork = unitofWork;
    }

    public async Task<Artist> CreateArtist(Artist artist)
    {
        await _unitOfWork.ArtistRepository.AddAsync(artist);
        await _unitOfWork.SaveAsync();
        return artist;
    }

    // Implementación de los demás métodos...
}
```

### Controlador de API

Implementa el controlador para exponer los endpoints:

```csharp
namespace Discoteque.API.Controller
{
    [Route("[controller]")]
    [ApiController]
    public class ArtistsController : ControllerBase
    {
        private readonly IArtistsService _artistsService;
        
        public ArtistsController(IArtistsService artistsService)
        {
            _artistsService = artistsService;
        }

        [HttpGet]
        public async Task<IActionResult> Get() {
            var artists = await _artistsService.GetArtistsAsync();
            return Ok(artists);
        }
    }
}
```

## Patrón Unit of Work

El patrón Unit of Work gestiona las transacciones y el ciclo de vida de las entidades:

```csharp
public interface IUnitOfWork
{
    IRepository<int, Artist> ArtistRepository{get;}
    Task SaveAsync();
}
```

La implementación completa incluye manejo de transacciones y disposición de recursos.

### Datos de Prueba

Agrega datos iniciales para pruebas:

```csharp
using (var scope = app.Services.CreateScope())
{
    var artistService = scope.ServiceProvider.GetRequiredService<IArtistsService>();
    
    await artistService.CreateArtist(new Artist{
        Name = "Karol G",
        Label = "Universal",
        IsOnTour = true
    });

    await artistService.CreateArtist(new Artist{
        Name = "Juanes",
        Label = "SONY BMG",
        IsOnTour = true
    });
}
```

## Configuración de Pruebas

Prepara el proyecto de pruebas:

```bash
dotnet new classlib -o Discoteque.Tests
dotnet sln add Discoteque.Tests/
dotnet add Discoteque.Tests reference Discoteque.Business/Discoteque.Business.csproj

# Paquetes de pruebas
dotnet add package Microsoft.NET.Test.Sdk --version 17.6.3
dotnet add package MSTest.TestAdapter --version 3.1.1
dotnet add package MSTest.TestFramework --version 3.1.1
dotnet add package Coverlet.collector --version 6.0.0
dotnet add package NSubstitute
dotnet add package NSubstitute.Analyzers.CSharp
```
