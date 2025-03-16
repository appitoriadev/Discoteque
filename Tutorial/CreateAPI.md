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
- **Tests**: Pruebas unitarias y de integración

```bash
# Crear la estructura base
dotnet new sln -n Discoteque
dotnet new classlib -o Discoteque.Business
dotnet new webapi -o Discoteque.API
dotnet new classlib -o Discoteque.Data
dotnet new classlib -o Discoteque.Tests

# Agregar proyectos a la solución
dotnet sln add Discoteque.API/
dotnet sln add Discoteque.Business/
dotnet sln add Discoteque.Data/
dotnet sln add Discoteque.Tests/
```

### Verificación de la Estructura

Confirma que la estructura se creó correctamente:

```bash
ls -a
# Estructura esperada:
# .                       Discoteque.API          Discoteque.sln
# ..                      Discoteque.Business     Discoteque.Data
# Discoteque.Tests
```

Compila la solución para verificar la integridad:

```bash
dotnet build
```

### Configuración de Dependencias

Instala los paquetes necesarios para cada proyecto:

```bash
# API Project
cd Discoteque.API/
dotnet add package Microsoft.AspNetCore.OpenApi -v 9.0.2
dotnet add package Microsoft.EntityFrameworkCore.Design -v 9.0.2
dotnet add package Swashbuckle.AspNetCore -v 7.3.1

# Data Project
cd ../Discoteque.Data/
dotnet add package Microsoft.EntityFrameworkCore -v 9.0.2
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL -v 9.0.3

# Business Project
cd ../Discoteque.Business/
dotnet add package Microsoft.Extensions.DependencyInjection.Abstractions -v 9.0.2

# Tests Project
cd ../Discoteque.Tests/
dotnet add package Microsoft.NET.Test.Sdk --version 17.6.3
dotnet add package MSTest.TestAdapter --version 3.1.1
dotnet add package MSTest.TestFramework --version 3.1.1
dotnet add package Coverlet.collector --version 6.0.0
dotnet add package NSubstitute
dotnet add package NSubstitute.Analyzers.CSharp
```

### Referencias entre Proyectos

Establece las dependencias entre proyectos:

```bash
# API depende de Business y Data
dotnet add Discoteque.API reference Discoteque.Business/Discoteque.Business.csproj
dotnet add Discoteque.API reference Discoteque.Data/Discoteque.Data.csproj

# Business depende de Data
dotnet add Discoteque.Business reference Discoteque.Data/Discoteque.Data.csproj

# Tests depende de Business y Data
dotnet add Discoteque.Tests reference Discoteque.Business/Discoteque.Business.csproj
dotnet add Discoteque.Tests reference Discoteque.Data/Discoteque.Data.csproj
```

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
    public virtual ICollection<Album> Albums { get; set; }
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
    public decimal Cost { get; set; }
    public int ArtistId { get; set; }
    public virtual Artist Artist { get; set; }
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

4. **Song.cs**: Modelo para canciones

```csharp
namespace Discoteque.Data.Models;

public class Song: BaseEntity<int>
{
    public string Name { get; set; }
    public int AlbumId { get; set; }
    public virtual Album Album { get; set; }
    public TimeSpan Duration { get; set; }
}
```

5. **Tour.cs**: Modelo para giras

```csharp
namespace Discoteque.Data.Models;

public class Tour: BaseEntity<int>
{
    public string Name { get; set; }
    public int ArtistId { get; set; }
    public virtual Artist Artist { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string City { get; set; }
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
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }

    public DbSet<Artist> Artists { get; set; } = null!;
    public DbSet<Album> Albums { get; set; } = null!;
    public DbSet<Song> Songs { get; set; } = null!;
    public DbSet<Tour> Tours { get; set; } = null!;
}
```

## Implementación de Servicios

### Interfaces de Servicio

Crea las interfaces en `Discoteque.Business/IServices/`:

1. **IArtistsService.cs**:
```csharp
namespace Discoteque.Business.IServices;

public interface IArtistsService
{
    Task<IEnumerable<Artist>> GetArtistsAsync();
    Task<Artist> GetById(int id);
    Task<Artist> CreateArtist(Artist artist);
    Task<Artist> UpdateArtist(Artist artist);
    Task DeleteArtist(int id);
}
```

2. **IAlbumService.cs**:
```csharp
namespace Discoteque.Business.IServices;

public interface IAlbumService
{
    Task<IEnumerable<Album>> GetAlbumsAsync();
    Task<Album> GetById(int id);
    Task<Album> CreateAlbum(Album album);
    Task<Album> UpdateAlbum(Album album);
    Task DeleteAlbum(int id);
}
```

3. **ISongService.cs**:
```csharp
namespace Discoteque.Business.IServices;

public interface ISongService
{
    Task<IEnumerable<Song>> GetSongsAsync();
    Task<Song> GetById(int id);
    Task<Song> CreateSong(Song song);
    Task<Song> UpdateSong(Song song);
    Task DeleteSong(int id);
}
```

4. **ITourService.cs**:
```csharp
namespace Discoteque.Business.IServices;

public interface ITourService
{
    Task<IEnumerable<Tour>> GetToursAsync();
    Task<Tour> GetById(int id);
    Task<Tour> CreateTour(Tour tour);
    Task<Tour> UpdateTour(Tour tour);
    Task DeleteTour(int id);
}
```

### Implementación de Servicios

Implementa los servicios en `Discoteque.Business/Services/`:

1. **ArtistsService.cs**:
```csharp
namespace Discoteque.Business.Services;

public class ArtistsService : IArtistsService
{
    private readonly IUnitOfWork _unitOfWork;

    public ArtistsService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<Artist>> GetArtistsAsync()
    {
        return await _unitOfWork.ArtistRepository.GetAllAsync();
    }

    public async Task<Artist> GetById(int id)
    {
        return await _unitOfWork.ArtistRepository.GetByIdAsync(id);
    }

    public async Task<Artist> CreateArtist(Artist artist)
    {
        await _unitOfWork.ArtistRepository.AddAsync(artist);
        await _unitOfWork.SaveAsync();
        return artist;
    }

    public async Task<Artist> UpdateArtist(Artist artist)
    {
        await _unitOfWork.ArtistRepository.UpdateAsync(artist);
        await _unitOfWork.SaveAsync();
        return artist;
    }

    public async Task DeleteArtist(int id)
    {
        await _unitOfWork.ArtistRepository.DeleteAsync(id);
        await _unitOfWork.SaveAsync();
    }
}
```

## Implementación de Controladores

Crea los controladores en `Discoteque.API/Controllers/`:

1. **ArtistsController.cs**:
```csharp
namespace Discoteque.API.Controllers;

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
    public async Task<IActionResult> Get()
    {
        var artists = await _artistsService.GetArtistsAsync();
        return Ok(artists);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var artist = await _artistsService.GetById(id);
        if (artist == null)
            return NotFound();
        return Ok(artist);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Artist artist)
    {
        var created = await _artistsService.CreateArtist(artist);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] Artist artist)
    {
        if (id != artist.Id)
            return BadRequest();
        var updated = await _artistsService.UpdateArtist(artist);
        return Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _artistsService.DeleteArtist(id);
        return NoContent();
    }
}
```

## Configuración de la API

### Program.cs

```csharp
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add DbContext
builder.Services.AddDbContext<DiscotequeContext>(
    opt => {
        opt.UseNpgsql(builder.Configuration.GetConnectionString("DiscotequeDatabase"));
    }    
);

// Add Services
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IArtistsService, ArtistsService>();
builder.Services.AddScoped<IAlbumService, AlbumService>();
builder.Services.AddScoped<ISongService, SongService>();
builder.Services.AddScoped<ITourService, TourService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

### appsettings.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DiscotequeDatabase": "Host=localhost;Username=postgres;Password=postgres;Database=discoteque;Port=5432"
  }
}
```

## Pruebas

### AlbumTests.cs

```csharp
namespace Discoteque.Tests;

[TestClass]
public class AlbumTests
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAlbumService _albumService;

    public AlbumTests()
    {
        var services = new ServiceCollection();
        services.AddDbContext<DiscotequeContext>(options =>
            options.UseInMemoryDatabase("TestDb"));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IAlbumService, AlbumService>();

        var serviceProvider = services.BuildServiceProvider();
        _unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();
        _albumService = serviceProvider.GetRequiredService<IAlbumService>();
    }

    [TestMethod]
    public async Task CreateAlbum_ShouldSucceed()
    {
        // Arrange
        var album = new Album
        {
            Name = "Test Album",
            Year = 2024,
            Genre = Genres.Rock,
            Cost = 10000
        };

        // Act
        var result = await _albumService.CreateAlbum(album);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(album.Name, result.Name);
    }
}
```

## Ejecución del Proyecto

1. Asegúrate de tener PostgreSQL instalado y corriendo localmente
2. Configura la cadena de conexión en `appsettings.json`
3. Ejecuta las migraciones:
```bash
cd Discoteque.API
dotnet ef database update
```
4. Inicia la API:
```bash
dotnet run
```

La API estará disponible en:
- https://localhost:7044
- http://localhost:5044
- Swagger UI: http://localhost:5044/swagger/index.html

> **Nota**: Los puertos pueden variar según tu configuración local. Si los puertos mencionados están en uso, .NET asignará automáticamente los siguientes puertos disponibles.
