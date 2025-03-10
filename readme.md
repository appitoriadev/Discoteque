# Discoteque

## Syllabus del bootcamp

### .Net Backend Bootcamp

**Instructor:** Luis Robles (Pioneras, WWC)
**Coordinadoras:** Girlesa Quintero (WWC), Kaky Rodríguez (Pioneras)
**Tutores, mentores:** David Arias (WWC), María Camila Gómez (Pioneras), Laura Velez (Globant), Pablo Uribe (Globant)

## Abstract

En este bootcamp vamos a construir un API RESTful Async con Repositorio Genérico utilizando Linq para buscar datos y listas. El objeto de este bootcamp es iniciar a los desarrolladores en el mundo del backend para .Net y que podamos contar con más profesionales en el área capaces de desempeñarse en cualquier ámbito y poder saltar a cualquier otro lenguaje orientado a objetos de alto nivel compilado, bien sea Java, Visual Basic .net, C, C++ o Ruby y Python sin que la curva de aprendizaje sea una línea vertical.

Para esto se creará un API Restful por capas en C# utilizando Linq y Entity Framework, con el patrón de repositorio genérico. Se contará con un Frontend en JS ya construido para utilizar el API y una base de datos ya poblada para buscar los datos.

La comunicación será constante y utilizaremos Discord para coordinar las actividades dentro y fuera del aula de clase, así como adelantar parte de la materia. El instructor tendrá office hours durante la semana para aclarar dudas y organizar los alumnos. Los mentores dependiendo de su disponibilidad, también tendrán office hours durante la semana.

## Estructura del bootcamp

### Requerimientos

Este bootcamp tiene un nivel de dificultad medio alto. Por lo cual se requiere tener los conceptos básicos de programación dominados:

- Estructuras de datos

- Estructuras de control

- Clases, objetos y tipos de datos fuertemente tipados.

Se requiere tener un nivel de inglés medio - alto.

Se requiere tener un computador con las siguientes características mínimas:

- Al menos 4 gbs de RAM

- CPU i3 de 5ta generación o Ryzen 3 de 2da generación

- SSD es preferible

- El OS no es una restricción. En caso de tener Linux deben informar que distribución tienen para asegurarnos la compatibilidad con el framework .Net

Para iniciar el bootcamp los siguientes programas y servicios deben estar instalados en el computador:

- El OS debe tener la última actualización

- Al menos 20 gbs de espacio libre en disco para las instalaciones de los programas y contener el set de datos.

- .net framework 7.0 

- Node JS

- SQL Server o Postgres. Revisar cual funciona con entity framework y cual es más liviana.

- Visual Studio Code con las siguientes extensiones

    * C#
    * Intellisense
    * (Incluir las extensiones que lo convierten en un VS Pro)
    * Codesnap
    * Git Lens

- Visual Studio Community

- Git

- Git Desktop

- Nu Get Packages:

    * Entity Framework
    * Json

#Descripción de cada día de instrucción


#### Día 1 - Sábado 8 de julio

- Historia del lenguaje, como funciona (CIL) y su flexibilidad para construir cualquier tipo de aplicación.

- Uso de la consola de comandos. Configuración del terminal con Oh My Posh, y FIG. Verificación que todo esté correctamente instalado.

- Clonación de los repositorios relacionados (base de datos, frontend en React). Asegurar que todo funcione correctamente.

- Creación del proyecto de WEB API a través de la consola. Hacer una breve demostración de cómo se haría a través de visual studio community.

- Comprobar que el servicio esté funcionando.

- Llevar el repositorio a Git y hacer el primer commit.

#### Día 2 - Sábado 15 de julio

- Crear la forma del repositorio genérico

- Entity Framework

- Explicar el patrón y las interfaces. 

- Explicar los tipos genéricos

- Explicar brevemente herencia.

#### Día 3 - Sábado 22 de julio

- Crear la conexión a BD local

- Traer los primeros registros de la BD y devolverlos serializados en un JSON

- Explicar la capa de negocios

- Explicar los distintos formatos de arquitectura

#### Día 4 - Sábado 29 de julio

- “El peligro de los microservicios”

- Gitflow

- Naming

- Codificación de página de caracteres

- Tools

- Home Brew

- Tasking and Async and Await

#### Día 5 - Sábado 12 de agosto (el 5 de agosto es puente, por lo tanto no habrá actividades este día)

- Hacer búsquedas con Linq. 

- Colecciones y listas.

- Hacer búsquedas con Linq en listas y colecciones

#### Día 6 - Sábado 26 de agosto (el 19 de agosto es puente, por lo tanto no habrá actividades este día)

- Pruebas unitarias

- Mocking

#### Día 7 - Sábado 2 de septiembre

- Refactorización

- Rutas de aprendizaje para el futuro

- Comparación con lenguajes

- Integración

- Docker y orquestadores

#### Día 8 - Sábado 9 de septiembre

- Graduación 

# Dotnet Bootcamp Women Who Code 2023: The discoteque

https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-7.0&tabs=visual-studio

# Default commands

```shell
# bash terminal
root@home: git config pull.rebase true
root@home: git pull
root@home: git push
root@home: git push set-upstream origin main
```

# Correct COnfiguration of the project

First we are going to delete everything but the `.git` folder and the `.gitignore`

# Setup Debugging

```shell
# bash terminal
root@home: dotnet new sln -n Discoteque
root@home: dotnet new classlib -o Discoteque.Business
root@home: dotnet new webapi -o Discoteque.API
root@home: dotnet new classlib -o Discoteque.Data
root@home: dotnet new classlib -o Discoteque.Business
root@home: dotnet sln add Discoteque.API/
root@home: dotnet sln add Discoteque.Business/
root@home: dotnet sln add Discoteque.Data/
root@home: ls
dotnet dotnet build
```

Once this is done you should receive a message like this

```shell
# begins output
MSBuild version 17.6.3+07e294721 for .NET
  Determining projects to restore...
  All projects are up-to-date for restore.
  Discoteque.Business -> /Users/dracvs/Projects/Pioneras/Discoteque/Discoteque.Business/bin/Debug/net7.0/Discoteque.Business.dll
  Discoteque.Data -> /Users/dracvs/Projects/Pioneras/Discoteque/Discoteque.Data/bin/Debug/net7.0/Discoteque.Data.dll
  Discoteque.API -> /Users/dracvs/Projects/Pioneras/Discoteque/Discoteque.API/bin/Debug/net7.0/Discoteque.API.dll

Build succeeded.
    0 Warning(s)
    0 Error(s)

Time Elapsed 00:00:02.17
```

```shell
# bash terminal
root@home: cd Discoteque.API/
root@home: dotnet add package Microsoft.EntityFrameworkCore.InMemory
root@home: cd Discoteque.Data/
root@home: dotnet add package Microsoft.EntityFrameworkCore.InMemory
```

On the debug tab on vs CODE, whilst you have the SLN open click on create a debug file. It will create a `launch.json` and a `tasks.json` 

# Code instructions

- Create a folder called Models in Discoteque.Data
- Add a baseEntity, Artist and Album Model

```csharp
// Discoteque.Data/Models/BaseEntity.cs
namespace Discoteque.Data.Models;

public class BaseEntity<TId> where TId : struct
{
    public TId Id { get; set; }
}
```

```csharp
// Discoteque.Data/Models/Artist.cs
namespace Discoteque.Data.Models;

public class Artist: BaseEntity<int>
{
    public string Name { get; set; }
    public string Label { get; set; }
    public bool IsOnTour { get; set; }
}
```

```csharp
// Discoteque.Data/Models/Albums.cs
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

- Create a DBContext

```csharp
// Discoteque.Data/DiscotequeContext.cs
using Discoteque.Data.Models;
using Microsoft.EntityFrameworkCore;

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

- Add a reference to the data project

```shell
# Bash terminal
root@home: dotnet add Discoteque.API reference Discoteque.Data/Discoteque.Data.csproj
root@home: dotnet add Discoteque.API reference Discoteque.Business/Discoteque.Busin
ess.csproj
root@home dotnet add Discoteque.Business/ reference Discoteque.Data/Discoteque.Data.csproj
```

- Register the Database Context

```csharp
// Discoteque.API/Program.cs
using Microsoft.EntityFrameworkCore;
using Discoteque.Data;


builder.Services.AddDbContext<DiscotequeContext>(
    opt => opt.UseInMemoryDatabase("Discoteque")
);

```

# Scaffold a Basic Controller

```shell
# bash Terminal
cd Discoteque.API
root@home: dotnet add package Microsoft.VisualStudio.Web.CodeGeneration.Design -v 7.0.7
root@home: dotnet add package Microsoft.EntityFrameworkCore.Design -v 7.0.7
root@home: dotnet add package Microsoft.EntityFrameworkCore.SqlServer -v 7.0.7
root@home: dotnet tool uninstall -g dotnet-aspnet-codegenerator
root@home: dotnet tool install -g dotnet-aspnet-codegenerator
root@home: dotnet tool list -g
root@home: dotnet tool update -g dotnet-ef
root@home: dotnet add package  Microsoft.EntityFrameworkCore.Tools --version  7.0.7

cd Discoteque.Data

# Create the basic Scaffold
root@home: cd Discoteque.Api/
root@home: dotnet-aspnet-codegenerator  Controllers -name ArtistsController -async -api  -outDir Controller --noViews
```

# Create a Business Layer Service

- Create a IService Folder in Data.Business
- Create Service folder in Data.Business
- Create the first interface for Artists

```Csharp
// Discoteque.Business/IService/IArtistsService.cs
using System.Collections;
using System;
using Discoteque.Data.Models;

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

```csharp
// Discoteque.Business/Service/ArtistsService.cs
using Discoteque.Business.IServices;
using Discoteque.Data.Models;

namespace Discoteque.Business.Services;

public class ArtistsService : IArtistsService
{
    public Task<IEnumerable<Artist>> GetArtistsAsync()
    {
        throw new NotImplementedException();
    }

    Task IArtistsService.AddAsync(Artist artist)
    {
        throw new NotImplementedException();
    }

    Task<IEnumerable<Artist>> IArtistsService.GetById(long id)
    {
        throw new NotImplementedException();
    }

    Task IArtistsService.UpdateAsync(Artist artist)
    {
        throw new NotImplementedException();
    }
}

```

# Update the Controller to comunicate with the Business Layer

```csharp

using Microsoft.AspNetCore.Mvc;
using Discoteque.Data;
using Discoteque.Business.IServices;

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

Update the program.cs to contain the references to the newly created business layer

```csharp
// Program.cs
builder.Services.AddScoped<IArtistsService, ArtistsService>();
```

At this point if set a break point into the newly created controller, we can see our app working with debugging correctly functioning in VS Code.

# Creating the Unit of Work

- Create a Repository interface and its respective class in the root of Discoteque.Data

```csharp
// Discotque.Data/IRepository.cs
using System.Linq.Expressions;
using Discoteque.Data.Models;

namespace Discoteque.Data.IRepositories
{

    public interface IRepository<Tid, TEntity>
    where Tid : struct
    where TEntity : BaseEntity<Tid>
    {
        Task AddAsync(TEntity entity);
        Task<TEntity> FindAsync(Tid id);
        Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> filter =  null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string inlcudeProperties = "");
        Task Update(TEntity entity);
        Task Delete(TEntity entity);
        Task Delete(Tid id);
    }
}
```

```csharp
// Discotque.Data/Repository.cs
using System.Linq.Expressions;
using Discoteque.Data.IRepositories;
using Discoteque.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Discoteque.Data.Repository;

public class Repository<Tid, TEntity> : IRepository<Tid, TEntity>
where Tid : struct
where TEntity : BaseEntity<Tid>
{
    internal DiscotequeContext _context;
    internal DbSet<TEntity> _dbSet;

    public Repository(DiscotequeContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }

    public virtual async Task<TEntity> FindAsync(Tid id)
    {
        return await _dbSet.FindAsync(id);
    }

    public virtual async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "")
    {
        IQueryable<TEntity> query = _dbSet;
        if(filter is not null)
        {
            query = query.Where(filter);
        }

        foreach (var includeProperty in includeProperties.Split(
            new char[] {','}, StringSplitOptions.RemoveEmptyEntries))
        {
            query = query.Include(includeProperty);
        }

        if(orderBy is not null)
        {
            return await orderBy(query).ToListAsync();
        }
        else
        {
            return await query.ToListAsync();
        }
    }

    public virtual async Task AddAsync(TEntity entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public virtual async Task Delete(TEntity entity)
    {
        if(_context.Entry(entity).State == EntityState.Detached)
        {
            _dbSet.Attach(entity);                
        }
        _dbSet.Remove(entity);
    }

    public virtual async Task Delete(Tid id)
    {
        TEntity entitToDetelete = await _dbSet.FindAsync(id);
        Delete(entitToDetelete);
        
        
    }

    public virtual async Task Update(TEntity entity)
    {
        _dbSet.Attach(entity);
        _context.Entry(entity).State = EntityState.Modified;
    }
}
```

```csharp
// Discotque.Data/IUnitOfWork.cs
using Discoteque.Data.Models;
using Discoteque.Data.IRepositories;

namespace Discoteque.Data;

public interface IUnitOfWork
{
    IRepository<int, Artist> ArtistRepository{get;}
    Task SaveAsync();
}

```

```csharp
// Discotque.Data/UnitOfWork.cs
using Microsoft.EntityFrameworkCore;
using Discoteque.Data.Models;
using Discoteque.Data.IRepositories;
using Discoteque.Data.Repository;

namespace Discoteque.Data;

public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly DiscotequeContext _context;
    private bool _disposed = false;
    private IRepository<int, Artist> _artistRepository;

    public UnitOfWork(DiscotequeContext context)
    {
        _context = context;
    }

    public IRepository<int, Artist> ArtistRepository
    {
        get 
        {
            if (_artistRepository is null)
            {
                _artistRepository = new Repository<int, Artist>(_context);
            }
            return _artistRepository;
        }
    }

    public async Task SaveAsync()
    {
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            ex.Entries.Single().Reload();
        }
    }
    #region IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if(!_disposed)
            {
                if(disposing)
                {
                    _context.DisposeAsync();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    
}

```

After all of this is said and done, we update everything in our Program.cs.

```csharp
builder.Services.AddDbContext<DiscotequeContext>(
    opt => opt.UseInMemoryDatabase("Discoteque")
);
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IArtistsService, ArtistsService>();
```

And we update our ArtistService.cs so it uses the Unit of Work

```csharp
using Discoteque.Business.IServices;
using Discoteque.Data;
using Discoteque.Data.Models;

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

    public async Task<IEnumerable<Artist>> GetArtistsAsync()
    {
        return await _unitOfWork.ArtistRepository.GetAllAsync();
    }

    public async Task<Artist> GetById(int id)
    {
        return await _unitOfWork.ArtistRepository.FindAsync(id);
    }

    public async Task<Artist> UpdateArtist(Artist artist)
    {
        await _unitOfWork.ArtistRepository.Update(artist);
        await _unitOfWork.SaveAsync();
        return artist;

    }
}

```

Then we add a small initializer

```csharp
// program.cs
using (var scope = app.Services.CreateScope())
{
    var artistService = scope.ServiceProvider.GetRequiredService<IArtistsService>();
    
    await artistService.CreateArtist(new Discoteque.Data.Models.Artist{
        Name = "Karol G",
        Label = "Universal",
        IsOnTour = true
    });

    await artistService.CreateArtist(new Discoteque.Data.Models.Artist{
        Name = "Juanes",
        Label = "SONY BMG",
        IsOnTour = true
    });
}
```

```shell
# bash terminal
root@home: dotnet new classlib -o Discoteque.Tests
root@home: dotnet sln add Discoteque.Tests/
root@home: dotnet add Discoteque.Tests reference Discoteque.Business/Discoteque.Business.csproj

# NuGet Library inclusion
root@home: dotnet add package Microsoft.NET.Test.Sdk --version 17.6.3
root@home: dotnet add package MSTest.TestAdapter --version 3.1.1
root@home: dotnet add package MSTest.TestFramework --version 3.1.1
root@home: dotnet add package Coverlet.collector --version 6.0.0
root@home: dotnet add package Moq --version 4.18.4

```
