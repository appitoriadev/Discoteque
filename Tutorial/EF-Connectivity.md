# 🔌 Conectando Entity Framework Core con PostgreSQL

## 🎯 Objetivo

En este tutorial, aprenderás a configurar Entity Framework Core para trabajar con PostgreSQL en tu aplicación .NET. Veremos cómo:
- Configurar la conexión a la base de datos
- Crear y aplicar migraciones
- Implementar el patrón Repository y Unit of Work
- Manejar la configuración en diferentes entornos

## 📦 Requisitos Previos

Asegúrate de tener instalado:
- .NET SDK
- PostgreSQL (si no usas Docker)
- Herramienta de EF Core:
  ```bash
  dotnet tool install --global dotnet-ef
  ```

## 🛠️ Configuración del Proyecto

### 1. Agregar Paquetes NuGet

En el proyecto `Discoteque.Data`, necesitas agregar los siguientes paquetes:

```xml
<ItemGroup>
    <PackageReference Include="Microsoft.EntityFrameworkCore" Version="9.0.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="9.0.0" />
    <PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="9.0.0" />
</ItemGroup>
```

### 2. Configurar el DbContext

Crea el archivo `DiscotequeContext.cs` en el proyecto `Discoteque.Data`:

```csharp
using Microsoft.EntityFrameworkCore;
using Discoteque.Data.Models;

namespace Discoteque.Data
{
    public class DiscotequeContext : DbContext
    {
        public DiscotequeContext(DbContextOptions<DiscotequeContext> options)
            : base(options)
        {
        }

        public DbSet<Artist> Artists { get; set; }
        public DbSet<Album> Albums { get; set; }
        public DbSet<Song> Songs { get; set; }
        public DbSet<Tour> Tours { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuraciones de entidades
            modelBuilder.Entity<Artist>()
                .HasMany(a => a.Albums)
                .WithOne(alb => alb.Artist)
                .HasForeignKey(alb => alb.ArtistId);

            modelBuilder.Entity<Album>()
                .HasMany(a => a.Songs)
                .WithOne(s => s.Album)
                .HasForeignKey(s => s.AlbumId);
        }
    }
}
```

### 3. Configurar el DesignTimeDbContextFactory

Para que las migraciones funcionen correctamente, crea el archivo `DesignTimeDbContextFactory.cs`:

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Discoteque.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<DiscotequeContext>
    {
        public DiscotequeContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DiscotequeContext>();
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=discoteque;Username=tu_usuario;Password=tu_contraseña");

            return new DiscotequeContext(optionsBuilder.Options);
        }
    }
}
```

## 🔄 Implementando el Patrón Repository y Unit of Work

### 1. Crear la Interfaz IRepository

```csharp
public interface IRepository<T> where T : class
{
    Task<T> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
}
```

### 2. Implementar Repository

```csharp
public class Repository<T> : IRepository<T> where T : class
{
    protected readonly DiscotequeContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(DiscotequeContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(T entity)
    {
        _dbSet.Remove(entity);
        await _context.SaveChangesAsync();
    }
}
```

### 3. Crear la Interfaz IUnitOfWork

```csharp
public interface IUnitOfWork : IDisposable
{
    IRepository<Artist> Artists { get; }
    IRepository<Album> Albums { get; }
    IRepository<Song> Songs { get; }
    IRepository<Tour> Tours { get; }
    Task<int> SaveChangesAsync();
}
```

### 4. Implementar UnitOfWork

```csharp
public class UnitOfWork : IUnitOfWork
{
    private readonly DiscotequeContext _context;
    private IRepository<Artist> _artists;
    private IRepository<Album> _albums;
    private IRepository<Song> _songs;
    private IRepository<Tour> _tours;

    public UnitOfWork(DiscotequeContext context)
    {
        _context = context;
    }

    public IRepository<Artist> Artists => _artists ??= new Repository<Artist>(_context);
    public IRepository<Album> Albums => _albums ??= new Repository<Album>(_context);
    public IRepository<Song> Songs => _songs ??= new Repository<Song>(_context);
    public IRepository<Tour> Tours => _tours ??= new Repository<Tour>(_context);

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
```

## 🚀 Configuración en Program.cs

```csharp
// En Program.cs
builder.Services.AddDbContext<DiscotequeContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DiscotequeDatabase")));

// Registrar UnitOfWork
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
```

## 📝 Configuración en appsettings.json

```json
{
  "ConnectionStrings": {
    "DiscotequeDatabase": "Host=localhost;Port=5432;Database=discoteque;Username=tu_usuario;Password=tu_contraseña"
  }
}
```

## 🎮 Creando y Aplicando Migraciones

```bash
# Crear una migración
dotnet ef migrations add InitialCreate --project Discoteque.Data --startup-project Discoteque.API

# Aplicar migraciones
dotnet ef database update --project Discoteque.Data --startup-project Discoteque.API
```

## 🔒 Seguridad y Configuración

### 1. User Secrets para Desarrollo

```bash
# Inicializar user secrets
dotnet user-secrets init --project Discoteque.API

# Agregar cadena de conexión
dotnet user-secrets set "ConnectionStrings:DiscotequeDatabase" "tu_cadena_de_conexion" --project Discoteque.API
```

### 2. Variables de Entorno

Para producción, usa variables de entorno:

```bash
export ConnectionStrings__DiscotequeDatabase="Host=tu_host;Port=5432;Database=discoteque;Username=tu_usuario;Password=tu_contraseña"
```

## 🧪 Verificando la Conexión

Puedes verificar la conexión agregando este código en `Program.cs`:

```csharp
try
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<DiscotequeContext>();
    await context.Database.EnsureCreatedAsync();
    Console.WriteLine("✅ Conexión a la base de datos establecida correctamente");
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Error al conectar con la base de datos: {ex.Message}");
}
```

## 🎉 ¡Felicitaciones!

Has configurado exitosamente Entity Framework Core con PostgreSQL en tu aplicación. Ahora puedes:
- Crear y aplicar migraciones
- Usar el patrón Repository y Unit of Work
- Manejar la configuración de manera segura
- Trabajar con la base de datos de forma eficiente

### 📚 Recursos Adicionales

- [Documentación de Entity Framework Core](https://docs.microsoft.com/ef/core/)
- [Documentación de Npgsql](https://www.npgsql.org/efcore/)
- [Patrón Repository](https://docs.microsoft.com/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/infrastructure-persistence-layer-implementation-entity-framework-core)
