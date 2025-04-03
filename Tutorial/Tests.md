# Guía de Testing para Discoteque

Esta guía proporciona información detallada sobre cómo realizar pruebas en el proyecto Discoteque, cubriendo pruebas unitarias, notebooks de prueba y pruebas de API con Postman, incluyendo la autenticación JWT.

## 0. Configuración del Proyecto de Pruebas

### 0.1 Instalación de Paquetes NuGet

Para utilizar Moq y xUnit en el proyecto de pruebas, necesitas agregar los siguientes paquetes NuGet:

```bash
# Desde la línea de comandos en el directorio del proyecto de pruebas
dotnet add package xunit
dotnet add package xunit.runner.visualstudio
dotnet add package Moq
dotnet add package Microsoft.NET.Test.Sdk
```

Alternativamente, puedes agregar estas referencias directamente en el archivo `Discoteque.Tests.csproj`:

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsPackable>false</IsPackable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.9.0" />
    <PackageReference Include="Moq" Version="4.20.70" />
    <PackageReference Include="xunit" Version="2.7.0" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.5.7">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
    <PackageReference Include="coverlet.collector" Version="6.0.0">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
  </ItemGroup>

  <ItemGroup>
    <ProjectReference Include="..\Discoteque.Business\Discoteque.Business.csproj" />
    <ProjectReference Include="..\Discoteque.Data\Discoteque.Data.csproj" />
  </ItemGroup>

</Project>
```

### 0.2 Referencias a Proyectos

Asegúrate de que el proyecto de pruebas tenga referencias a los proyectos que contiene las clases que deseas probar:

```bash
# Desde la línea de comandos
dotnet add reference ../Discoteque.Business/Discoteque.Business.csproj
dotnet add reference ../Discoteque.Data/Discoteque.Data.csproj
```

### 0.3 Configuración de la Ejecución de Pruebas

Para configurar la ejecución de pruebas en Visual Studio Code, puedes crear un archivo `launch.json` en la carpeta `.vscode` con la siguiente configuración:

```json
{
    "version": "0.2.0",
    "configurations": [
        {
            "name": ".NET Core Test",
            "type": "coreclr",
            "request": "launch",
            "preLaunchTask": "build",
            "program": "${workspaceFolder}/Discoteque.Tests/bin/Debug/net9.0/Discoteque.Tests.dll",
            "args": [],
            "cwd": "${workspaceFolder}/Discoteque.Tests",
            "console": "internalConsole",
            "stopAtEntry": false
        }
    ]
}
```

## 1. Pruebas Unitarias (Discoteque.Tests)

### 1.1 Estructura del Proyecto de Pruebas

El proyecto `Discoteque.Tests` está configurado para realizar pruebas unitarias utilizando xUnit y Moq para mocking. La estructura básica de una clase de prueba es:

```csharp
using Xunit;
using Moq;
using Discoteque.Business.Services;
using Discoteque.Data;
using Discoteque.Data.Models;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

public class AlbumServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly AlbumService _albumService;
    
    public AlbumServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _albumService = new AlbumService(_unitOfWorkMock.Object);
    }
}
```

### 1.2 Creación de Pruebas Unitarias

Para crear una nueva prueba unitaria, sigue estos pasos:

1. Crea un método con el atributo `[Fact]`
2. Sigue el patrón AAA (Arrange-Act-Assert)
3. Utiliza Moq para mockear dependencias

Ejemplo de prueba para crear un álbum:

```csharp
[Fact]
public async Task CreateAlbum_ShouldReturnSuccess()
{
    // Arrange - Preparar los datos y mocks
    var album = new Album { Name = "Test Album", Year = 2023, Genre = Genres.Rock };
    var artist = new Artist { Id = 1, Name = "Test Artist" };
    
    _unitOfWorkMock.Setup(u => u.ArtistRepository.FindAsync(It.IsAny<int>()))
        .ReturnsAsync(artist);
    
    _unitOfWorkMock.Setup(u => u.AlbumRepository.AddAsync(It.IsAny<Album>()))
        .Returns(Task.CompletedTask);
    
    _unitOfWorkMock.Setup(u => u.SaveAsync())
        .Returns(Task.CompletedTask);

    // Act - Ejecutar la acción a probar
    var result = await _albumService.CreateAlbum(album);

    // Assert - Verificar el resultado
    Assert.NotNull(result);
    Assert.Equal(System.Net.HttpStatusCode.OK, result.StatusCode);
}
```

### 1.3 Pruebas de Autenticación JWT

Para probar la autenticación JWT, puedes crear una clase específica:

```csharp
using Xunit;
using Moq;
using Discoteque.Business.Services;
using Discoteque.Data;
using Discoteque.Data.Models;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Linq.Expressions;
using System.Collections.Generic;

public class AuthServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly AuthService _authService;
    private readonly string _testJwtKey = "your-256-bit-secret-your-256-bit-secret-your-256-bit-secret";

    public AuthServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _configurationMock = new Mock<IConfiguration>();
        
        var configurationSection = new Mock<IConfigurationSection>();
        configurationSection.Setup(x => x.Value).Returns(_testJwtKey);
        
        _configurationMock.Setup(x => x.GetSection("Jwt:Key"))
            .Returns(configurationSection.Object);
            
        _authService = new AuthService(_unitOfWorkMock.Object, _configurationMock.Object);
    }

    [Fact]
    public async Task Register_ShouldReturnAuthResponse()
    {
        // Arrange
        var user = new User { Username = "testuser", PasswordHash = "password", Email = "mail@test.test" };
        _unitOfWorkMock.Setup(u => u.UserRepository.FindAsync(It.IsAny<int>())).ReturnsAsync((User)null!);

        // Act
        var result = await _authService.Register(user);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Username, result.Username);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ShouldReturnToken()
    {
        // Arrange
        var user = new User { Username = "testuser", PasswordHash = "password", Email = "mail@test.test" };
        var loginRequest = new LoginRequest { Username = user.Username, Password = user.PasswordHash };
        
        _unitOfWorkMock.Setup(u => u.UserRepository.GetAllAsync(
            It.Is<Expression<Func<User, bool>>>(expr => expr.Compile()(user)),
            null!,
            ""))
            .ReturnsAsync(new List<User> { user });

        // Act
        var result = await _authService.Login(loginRequest);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Token);
        Assert.True(ValidateToken(result.Token));
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ShouldReturnNull()
    {
        // Arrange
        var loginRequest = new LoginRequest { Username = "invaliduser", Password = "wrongpassword" };
        
        _unitOfWorkMock.Setup(u => u.UserRepository.GetAllAsync(
            It.Is<Expression<Func<User, bool>>>(expr => expr.Compile()(new User { 
                Username = loginRequest.Username, 
                PasswordHash = "hashedpassword",
                Email = "test@test.com"
            })),
            null!,
            ""))
            .ReturnsAsync(new List<User>());

        // Act
        var result = await _authService.Login(loginRequest);

        // Assert
        Assert.Null(result);
    }

    private bool ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_testJwtKey);
        
        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            return true;
        }
        catch
        {
            return false;
        }
    }
}
```

### 1.4 Ejecutar Pruebas Unitarias

Para ejecutar las pruebas unitarias:

```bash
# Desde la línea de comandos
dotnet test Discoteque.Tests

# Para ver resultados detallados
dotnet test Discoteque.Tests -v n
```

## 2. Testing Notebooks (.NET Interactive)

El archivo `Dotnet_Testing_Notebooks.ipynb` en el directorio Testing permite realizar pruebas interactivas y experimentación con el código.

### 2.1 Configuración del Notebook

1. Instala la extensión .NET Interactive Notebooks en Visual Studio Code
2. Abre el archivo `Dotnet_Testing_Notebooks.ipynb`
3. Asegúrate de tener el kernel de C# seleccionado

### 2.2 Uso del Notebook

El notebook permite:
- Probar fragmentos de código de forma interactiva
- Visualizar resultados inmediatamente
- Documentar el proceso de prueba con markdown
- Experimentar con diferentes escenarios de prueba

Ejemplo de prueba de JWT:
```csharp
// Celda de código para pruebas de JWT
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

// Generar token de prueba
var tokenHandler = new JwtSecurityTokenHandler();
var key = Encoding.ASCII.GetBytes("tu_clave_secreta");
var tokenDescriptor = new SecurityTokenDescriptor
{
    Subject = new ClaimsIdentity(new[]
    {
        new Claim(ClaimTypes.Name, "testuser"),
        new Claim(ClaimTypes.Role, "admin")
    }),
    Expires = DateTime.UtcNow.AddDays(7),
    SigningCredentials = new SigningCredentials(
        new SymmetricSecurityKey(key),
        SecurityAlgorithms.HmacSha256Signature)
};

var token = tokenHandler.CreateToken(tokenDescriptor);
Console.WriteLine(tokenHandler.WriteToken(token));

// Validar token
var validationParameters = new TokenValidationParameters
{
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(key),
    ValidateIssuer = false,
    ValidateAudience = false,
    ClockSkew = TimeSpan.Zero
};

try
{
    var principal = tokenHandler.ValidateToken(tokenHandler.WriteToken(token), validationParameters, out SecurityToken validatedToken);
    Console.WriteLine("Token válido");
    Console.WriteLine($"Usuario: {principal.Identity.Name}");
}
catch (Exception ex)
{
    Console.WriteLine($"Token inválido: {ex.Message}");
}
```

## 3. Pruebas de API con Postman

La colección Postman se encuentra en `PostmanCollections/The Discoteque.postman_collection.json`.

### 3.1 Importar la Colección

1. Abre Postman
2. Haz clic en "Import"
3. Arrastra el archivo `The Discoteque.postman_collection.json` o selecciónalo desde el diálogo
4. Confirma la importación

### 3.2 Estructura de la Colección

La colección está organizada por recursos:

#### Autenticación
- POST /api/auth/login
- POST /api/auth/refresh-token
- POST /api/auth/logout

#### Albums (Protegidos)
- GET /api/album/getalbums
- GET /api/album/getalbumbyid
- GET /api/album/getalbumsbyyear
- GET /api/album/getalbumsbyyearrange
- GET /api/album/getalbumsbygenre
- GET /api/album/getalbumsbyartist
- POST /api/album/createalbum

#### Songs (Protegidos)
- GET /api/song/getsongs
- GET /api/tour/gettours

### 3.3 Configuración del Ambiente

1. Crea un nuevo ambiente en Postman
2. Configura las variables:
   ```json
   {
     "baseUrl": "http://localhost:5044",
     "apiVersion": "api",
     "token": "",
     "refreshToken": ""
   }
   ```

### 3.4 Scripts de Autenticación

Agrega este script en la pestaña "Tests" de la petición de login:

```javascript
pm.test("Login successful", function () {
    pm.response.to.have.status(200);
    var jsonData = pm.response.json();
    pm.expect(jsonData.token).to.exist;
    pm.environment.set("token", jsonData.token);
    pm.environment.set("refreshToken", jsonData.refreshToken);
});
```

### 3.5 Ejecutar Pruebas

1. Asegúrate de que la API esté corriendo localmente
2. Selecciona el ambiente configurado
3. Ejecuta primero la petición de login para obtener el token
4. Ejecuta las peticiones protegidas individualmente o usa la función "Run Collection"

## 4. Mejores Prácticas

### 4.1 Pruebas Unitarias
- Mantén las pruebas independientes entre sí
- Usa nombres descriptivos para los métodos de prueba
- Sigue el patrón AAA (Arrange-Act-Assert)
- Mockea las dependencias externas
- Prueba tanto casos exitosos como fallidos de autenticación
- Verifica la expiración de tokens
- Utiliza expresiones lambda para configurar los mocks de manera más precisa
- Maneja correctamente los valores nulos con el operador `!` cuando sea necesario

### 4.2 Notebooks
- Documenta el propósito de cada celda
- Limpia las variables entre pruebas
- Usa markdown para explicar el código
- Incluye ejemplos de generación y validación de tokens
- Prueba diferentes escenarios de autenticación

### 4.3 Postman
- Verifica los códigos de estado HTTP
- Valida el formato de las respuestas
- Usa variables de ambiente para tokens
- Documenta los casos de prueba
- Implementa scripts de autenticación automática
- Prueba el flujo completo de refresh token

## 5. Resolución de Problemas

### 5.1 Pruebas Unitarias
- Error de dependencias: Verifica las referencias en el archivo .csproj
- Fallos en los mocks: Revisa la configuración de Moq
- Errores de token: Verifica la configuración de JWT en appsettings.json
- Errores de compilación: Asegúrate de que los tipos genéricos estén correctamente especificados
- Errores de null: Utiliza el operador `!` para indicar que un valor no será null cuando el compilador lo requiera

### 5.2 Notebooks
- Kernel no responde: Reinicia el kernel
- Error de referencias: Verifica los using statements
- Errores de token: Verifica la clave secreta y configuración

### 5.3 Postman
- Error de conexión: Verifica que la API esté corriendo
- Error 404: Revisa las rutas de los endpoints
- Error en las respuestas: Valida el formato de los datos enviados
- Error 401: Verifica que el token sea válido y no haya expirado
- Error de refresh token: Verifica la configuración de expiración
