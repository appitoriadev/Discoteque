# Dotnet: The discoteque

Read more about the tutorial topics here:

[Tutorial: Create a controller-based web API with ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-9.0&tabs=visual-studio)

Now let's get our hands dirty. On the terminal you should go to a folder of your choosing to be able to create a new project. Usually we would recommend to create a folder inside of your `home` or `C:\` folder, let's call it `code`. You can do it all from your terminal like so.

Step 1. Open your terminal and input the following commands:

```bash
cd C:/ # This command will set you in your C:/ folder for windows. Avoid this command if you are in Linux or MacOs
mkdir code # All OS.
```

## Default commands

After you create your `code` folder you should direct all the following commands inside of it:

```bash
cd code/
mkdir Discoteque
cd Discoteque/ # This command helps you to enter a folder of your choosing. i.Ex: Discoteque
```

### Setup Debugging

```bash
# All the following commands will create all the application we will work on the tutorial.
dotnet new sln -n Discoteque
dotnet new classlib -o Discoteque.Business
dotnet new webapi -o Discoteque.API
dotnet new classlib -o Discoteque.Data
dotnet sln add Discoteque.API/
dotnet sln add Discoteque.Business/
dotnet sln add Discoteque.Data/
```

After you create all the projects and Web API you can verify that every project it's correct by listing and then building your solution:

```bash
ls -a
# You should see something like this:
.                       Discoteque.API          Discoteque.sln
..                      Discoteque.Business     Discoteque.Data
Discoteque.Tests
# Then you can run the following command to build your application
dotnet dotnet build
```

Once this is done you should receive a message like this

```bash
$ dotnet dotnet build
Restore complete (0.6s)
  Discoteque.Business succeeded (1.2s) → Discoteque.Business/bin/Debug/net9.0/Discoteque.Business.dll
  Discoteque.Data succeeded (1.2s) → Discoteque.Data/bin/Debug/net9.0/Discoteque.Data.dll
  Discoteque.API succeeded (1.3s) → Discoteque.API/bin/Debug/net9.0/Discoteque.API.dll

Build succeeded in 2.2s
```

Then you can start adding all the libraries needed in each project:

```bash
cd Discoteque.API/
dotnet add package Microsoft.EntityFrameworkCore.InMemory
cd .. # This command helps you to go back one folder
cd Discoteque.Data/
dotnet add package Microsoft.EntityFrameworkCore.InMemory
cd .. # This command helps you to go back one folder
```

Then you can go into the `code` folder and grab the `Discoteque` and drop it into vsCode. There you can see all your recently created projects.

Last step on the debug tab on vs CODE, whilst you have the solution (projects) open, click on `create a launch.json file`.
Once you have the file you can add in the same `.vscode` folder the files for `tasks.json` and `settings.json`. Each file should have the following code in them to be able to run the solution correctly:

First the **launch.json** file:

```json
{
    "version": "0.2.0",
    "configurations": [
        {
            "name": "Docker .NET Attach (Preview)",
            "type": "docker",
            "request": "attach",
            "preLaunchTask": "docker-run: debug",
            "platform": "netCore",
            "sourceFileMap": {
                "/src": "${workspaceFolder}"
            },
            "netCore": {
                "appProject": "${workspaceFolder}/Discoteque.API/Discoteque.API.csproj"
            }
        },
        {
            "name": ".NET Core Launch (web)",
            "type": "coreclr",
            "request": "launch",
            "preLaunchTask": "build",
            "program": "${workspaceFolder}/Discoteque.API/bin/Debug/net7.0/Discoteque.API.dll",
            "args": [],
            "cwd": "${workspaceFolder}/Discoteque.API",
            "stopAtEntry": false,
            "serverReadyAction": {
                "action": "openExternally",
                "pattern": "\\bNow listening on:\\s+(https?://\\S+)"
            },
            "env": {
                "ASPNETCORE_ENVIRONMENT": "Development"
            },
            "sourceFileMap": {
                "/Views": "${workspaceFolder}/Views"
            }
        },
        {
            "name": ".NET Core Attach",
            "type": "coreclr",
            "request": "attach"
        }
    ]
}
```

Second the **tasks.json** file:

```json
{
  "version": "2.0.0",
  "tasks": [
    {
      "label": "build",
      "command": "dotnet",
      "type": "process",
      "args": [
        "build",
        "${workspaceFolder}/Discoteque.sln",
        "/property:GenerateFullPaths=true",
        "/consoleloggerparameters:NoSummary"
      ],
      "problemMatcher": "$msCompile"
    },
    {
      "type": "docker-build",
      "label": "docker-build: debug",
      "dependsOn": ["build"],
      "dockerBuild": {
        "tag": "discoteque:dev",
        "target": "base",
        "dockerfile": "${workspaceFolder}/Discoteque.API/Dockerfile",
        "context": "${workspaceFolder}",
        "pull": true
      },
      "netCore": {
        "appProject": "${workspaceFolder}/Discoteque.API/Discoteque.API.csproj"
      }
    },
    {
      "type": "docker-run",
      "label": "docker-run: debug",
      "dependsOn": ["docker-build: debug"],
      "dockerRun": {},
      "netCore": {
        "appProject": "${workspaceFolder}/Discoteque.API/Discoteque.API.csproj",
        "enableDebugging": true
      }
    },
    {
      "label": "publish",
      "command": "dotnet",
      "type": "process",
      "args": [
        "publish",
        "${workspaceFolder}/Discoteque.sln",
        "/property:GenerateFullPaths=true",
        "/consoleloggerparameters:NoSummary"
      ],
      "problemMatcher": "$msCompile"
    },
    {
      "label": "watch",
      "command": "dotnet",
      "type": "process",
      "args": [
        "watch",
        "run",
        "--project",
        "${workspaceFolder}/Discoteque.API/Discoteque.API.csproj",
        "/property:GenerateFullPaths=true",
        "/consoleloggerparameters:NoSummary"
      ],
      "problemMatcher": "$msCompile",
      "options": { "cwd": "${workspaceFolder}/Discoteque.API/" }
    }
  ]
}
```

Lastly the **settings.json** file:

```json
{
    "dotnet.defaultSolution": "Discoteque.sln"
}
```

## Code instructions

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

```bash
dotnet add Discoteque.API reference Discoteque.Data/Discoteque.Data.csproj
dotnet add Discoteque.API reference Discoteque.Business/Discoteque.Business.csproj
dotnet add Discoteque.Business/ reference Discoteque.Data/Discoteque.Data.csproj
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

## Scaffold a Basic Controller

```bash
cd Discoteque.API
dotnet add package Microsoft.AspNetCore.OpenApi -v 9.0.2
dotnet add package Microsoft.EntityFrameworkCore.Design -v 9.0.2
dotnet add package Microsoft.EntityFrameworkCore.InMemory -v 9.0.2
dotnet add package Microsoft.EntityFrameworkCore.SqlServer -v 9.0.2
dotnet add package Microsoft.VisualStudio.Web.CodeGeneration.Design -v 9.0.0
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL -v 9.0.3
dotnet add package Swashbuckle.AspNetCore -v 7.3.1
dotnet tool uninstall -g dotnet-aspnet-codegenerator # If you don't have it it will say it couldn't find it.
dotnet tool install -g dotnet-aspnet-codegenerator
dotnet tool list -g
dotnet tool update -g dotnet-ef
dotnet add package Microsoft.EntityFrameworkCore.Tools -v 9.0.2
dotnet-aspnet-codegenerator  Controllers -name ArtistsController -async -api  -outDir Controller --noViews
```

## Create a Business Layer Service

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

## Update the Controller to comunicate with the Business Layer

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

Update the `Program.cs` to contain the references to the newly created business layer

```csharp
// Program.cs
builder.Services.AddScoped<IArtistsService, ArtistsService>();
```

At this point if set a break point into the newly created controller, we can see our app working with debugging correctly functioning in VS Code.

## Creating the Unit of Work

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

After all of this is said and done, we update everything in our `Program.cs`.

```csharp
builder.Services.AddDbContext<DiscotequeContext>(
    opt => opt.UseInMemoryDatabase("Discoteque")
);
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IArtistsService, ArtistsService>();
```

And we update our `ArtistService.cs` so it uses the Unit of Work

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

```bash
# bash terminal
dotnet new classlib -o Discoteque.Tests
dotnet sln add Discoteque.Tests/
dotnet add Discoteque.Tests reference Discoteque.Business/Discoteque.Business.csproj

# NuGet Library inclusion
dotnet add package Microsoft.NET.Test.Sdk --version 17.6.3
dotnet add package MSTest.TestAdapter --version 3.1.1
dotnet add package MSTest.TestFramework --version 3.1.1
dotnet add package Coverlet.collector --version 6.0.0
dotnet add package NSubstitute
dotnet add package NSubstitute.Analyzers.CSharp
```
