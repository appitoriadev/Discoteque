# Dotnet: The discoteque

## Requisitos Previos

En este repositorio podrás seguir un tutorial sobre cómo crear una aplicación .Net, contenerizarla y dockerizarla.
Primero necesitaremos realizar algunas instalaciones y validaciones para configurar correctamente nuestros entornos.

### 1. Instalaciones de .Net

Primero necesitarás descargar e instalar la versión más reciente de .Net:

[.Net Current Release](https://dotnet.microsoft.com/en-us/download)

Luego puedes verificar la versión instalada con:

```bash
# bash terminal
dotnet --version
```

### 2. Instalaciones de Docker

Revisa el siguiente enlace para descargar e instalar Docker en tu computadora:

[Docker Desktop](https://www.docker.com/)

Luego puedes verificar la versión instalada con:

```bash
# bash terminal
docker --version
```

### 3. Instalaciones de PostgreSQL (Opcional)

Si deseas ejecutar la aplicación sin Docker, necesitarás instalar PostgreSQL:

[PostgreSQL Downloads](https://www.postgresql.org/download/)

Verifica la instalación con:

```bash
# bash terminal
psql --version
```

### 4. Extensiones de VSCode

Una vez que tengas los programas instalados, deberías buscar en VSCode para agregar algunas extensiones necesarias:

1. [.Net Extension Pack](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.vscode-dotnet-pack)
2. [.NET Install Tool](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.vscode-dotnet-runtime)
3. [C# Dev Kit](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit)
4. [IntelliCode for C# Dev Kit](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.vscodeintellicode-csharp)
5. [C#](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp)
6. [Docker](https://marketplace.visualstudio.com/items?itemName=ms-azuretools.vscode-docker)
7. [PostgreSQL](https://marketplace.visualstudio.com/items?itemName=ckolkman.vscode-postgres)

### 5. Herramientas Adicionales

Para trabajar con Entity Framework Core, necesitarás instalar la herramienta de EF Core:

```bash
dotnet tool install --global dotnet-ef
```

## Estructura del Proyecto

El proyecto está organizado en las siguientes capas:

```
Discoteque/
├── Discoteque.API/        # API REST y configuración
├── Discoteque.Business/   # Lógica de negocio y servicios
├── Discoteque.Data/       # Acceso a datos y modelos
└── Discoteque.Tests/      # Pruebas unitarias
```

## Configuración del Entorno

### Variables de Entorno

Para desarrollo local, necesitarás configurar las siguientes variables de entorno:

```bash
# Para desarrollo local
ASPNETCORE_ENVIRONMENT=Development
ConnectionStrings__DiscotequeDatabase=Host=localhost;Port=5432;Database=discoteque;Username=tu_usuario;Password=tu_contraseña
```

### User Secrets (Desarrollo Local)

Para desarrollo local, puedes usar user secrets para almacenar información sensible:

```bash
dotnet user-secrets init --project Discoteque.API
dotnet user-secrets set "ConnectionStrings:DiscotequeDatabase" "tu_cadena_de_conexion" --project Discoteque.API
```

**¡Felicitaciones!** Has configurado exitosamente tu entorno de trabajo. ¡Ahora puedes comenzar con el tutorial que hemos preparado para ti!

[Ir a la creación de API...](Tutorial/CreateAPI.md)

[Ir a Conectividad...](Tutorial/EF-Connectivity.md)

[Ir a Dockerización...](Tutorial/Docker.md)