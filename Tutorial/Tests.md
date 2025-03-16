# Guía de Testing para Discoteque

Esta guía proporciona información detallada sobre cómo realizar pruebas en el proyecto Discoteque, cubriendo pruebas unitarias, notebooks de prueba y pruebas de API con Postman.

## 1. Pruebas Unitarias (Discoteque.Tests)

### 1.1 Estructura del Proyecto de Pruebas

El proyecto `Discoteque.Tests` está configurado para realizar pruebas unitarias utilizando MSTest y NSubstitute para mocking. La estructura básica de una clase de prueba es:

```csharp
[TestClass]
public class AlbumTests
{
    private readonly IRepository<int, Album> _albumRepository;
    private readonly IRepository<int, Artist> _artistRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAlbumService _albumService;
    
    public AlbumTests()
    {
        // Configuración inicial de las pruebas
        _albumRepository = Substitute.For<IRepository<int, Album>>();
        _artistRepository = Substitute.For<IRepository<int, Artist>>();
        _unitOfWork = Substitute.For<IUnitOfWork>();
        _albumService = new AlbumService(_unitOfWork);
    }
}
```

### 1.2 Creación de Pruebas Unitarias

Para crear una nueva prueba unitaria, sigue estos pasos:

1. Crea un método con el atributo `[TestMethod]`
2. Sigue el patrón AAA (Arrange-Act-Assert)
3. Utiliza NSubstitute para mockear dependencias

Ejemplo:

```csharp
[TestMethod]
public async Task IsAlbumCreatedCorrectly()
{
    // Arrange - Preparar los datos y mocks
    _artistRepository.FindAsync(1).Returns(Task.FromResult(new Artist()));
    _albumRepository.AddAsync(_correctAlbum).Returns(Task.FromResult(_correctAlbum));
    _unitOfWork.AlbumRepository.Returns(_albumRepository);
    _unitOfWork.ArtistRepository.Returns(_artistRepository);

    // Act - Ejecutar la acción a probar
    var newAlbum = await _albumService.CreateAlbum(_correctAlbum);

    // Assert - Verificar el resultado
    Assert.AreEqual(newAlbum.StatusCode, System.Net.HttpStatusCode.OK);
}
```

### 1.3 Ejecutar Pruebas Unitarias

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

Ejemplo de uso:
```csharp
// Celda de código para pruebas
using System.Linq;
using System.Text.RegularExpressions;

// Prueba de validación de nombres
string name = "El son de revolución";
var prohibitedWords = new List<string>(){"Revolución", "Poder","Amor","Guerra"};
var value = prohibitedWords.Any(keyword => 
    Regex.IsMatch(name, Regex.Escape(keyword), RegexOptions.IgnoreCase));
Console.WriteLine(value);
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

#### Albums
- GET /api/album/getalbums
- GET /api/album/getalbumbyid
- GET /api/album/getalbumsbyyear
- GET /api/album/getalbumsbyyearrange
- GET /api/album/getalbumsbygenre
- GET /api/album/getalbumsbyartist
- POST /api/album/createalbum

#### Songs
- GET /api/song/getsongs
- GET /api/tour/gettours

### 3.3 Configuración del Ambiente

1. Crea un nuevo ambiente en Postman
2. Configura las variables:
   ```json
   {
     "baseUrl": "http://localhost:5044",
     "apiVersion": "api"
   }
   ```

### 3.4 Ejecutar Pruebas

1. Asegúrate de que la API esté corriendo localmente
2. Selecciona el ambiente configurado
3. Ejecuta las peticiones individualmente o usa la función "Run Collection" para ejecutar todas las pruebas

## 4. Mejores Prácticas

### 4.1 Pruebas Unitarias
- Mantén las pruebas independientes entre sí
- Usa nombres descriptivos para los métodos de prueba
- Sigue el patrón AAA (Arrange-Act-Assert)
- Mockea las dependencias externas

### 4.2 Notebooks
- Documenta el propósito de cada celda
- Limpia las variables entre pruebas
- Usa markdown para explicar el código

### 4.3 Postman
- Verifica los códigos de estado HTTP
- Valida el formato de las respuestas
- Usa variables de ambiente
- Documenta los casos de prueba

## 5. Resolución de Problemas

### 5.1 Pruebas Unitarias
- Error de dependencias: Verifica las referencias en el archivo .csproj
- Fallos en los mocks: Revisa la configuración de NSubstitute

### 5.2 Notebooks
- Kernel no responde: Reinicia el kernel
- Error de referencias: Verifica los using statements

### 5.3 Postman
- Error de conexión: Verifica que la API esté corriendo
- Error 404: Revisa las rutas de los endpoints
- Error en las respuestas: Valida el formato de los datos enviados
