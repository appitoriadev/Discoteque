# 🐳 Dockerizando tu Primera Aplicación .NET

¡Hola! Si estás aquí, probablemente estés dando tus primeros pasos en el mundo del desarrollo de software moderno. En este tutorial, te guiaré a través del proceso de "dockerización" de una aplicación .NET, explicando cada concepto de manera amigable y práctica.

## 🌟 ¿Qué es Docker y por qué debería importarme?

Imagina que estás preparando una receta de cocina. Necesitas ingredientes específicos, utensilios particulares y seguir ciertos pasos. Docker es como tener una cocina portátil que ya viene con todo lo necesario para preparar tu receta, sin importar dónde estés.

En términos técnicos, Docker es una plataforma que permite crear, desplegar y ejecutar aplicaciones en contenedores. Un contenedor es como una caja que contiene todo lo que tu aplicación necesita para funcionar: el código, las dependencias, la configuración, etc.

### 🎯 Beneficios de usar Docker:

1. **Consistencia**: Tu aplicación funcionará igual en cualquier lugar
2. **Aislamiento**: Cada aplicación vive en su propio "mundo"
3. **Portabilidad**: Puedes mover tu aplicación fácilmente entre diferentes computadoras
4. **Escalabilidad**: Puedes crear múltiples copias de tu aplicación sin esfuerzo

## 🚀 Preparando tu Primera Aplicación Dockerizada

### 1. 📦 Estructura del Proyecto

Nuestra aplicación tiene una estructura en capas:
```
Discoteque/
├── Discoteque.API/        # La interfaz de usuario
├── Discoteque.Business/   # La lógica de negocio
├── Discoteque.Data/       # El acceso a datos
└── Discoteque.Tests/      # Las pruebas
```

### 2. 🐳 El Dockerfile

El Dockerfile es como una receta que le dice a Docker cómo construir tu aplicación. Vamos a analizarlo paso a paso:

```dockerfile
# 1. Imagen base para ejecutar la aplicación
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5044
ENV ASPNETCORE_URLS=http://+:5044

# 2. Imagen para compilar la aplicación
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# 3. Copiar archivos del proyecto
COPY ["Discoteque.sln", "."]
COPY ["Discoteque.API/Discoteque.API.csproj", "Discoteque.API/"]
COPY ["Discoteque.Business/Discoteque.Business.csproj", "Discoteque.Business/"]
COPY ["Discoteque.Data/Discoteque.Data.csproj", "Discoteque.Data/"]
COPY ["Discoteque.Tests/Discoteque.Tests.csproj", "Discoteque.Tests/"]

# 4. Restaurar dependencias
RUN dotnet restore

# 5. Copiar el código fuente
COPY . .

# 6. Instalar herramientas de Entity Framework
RUN dotnet tool install --global dotnet-ef
ENV PATH="${PATH}:/root/.dotnet/tools"

# 7. Compilar la aplicación
WORKDIR "/src/Discoteque.API"
RUN dotnet build "Discoteque.API.csproj" -c Release -o /app/build

# 8. Publicar la aplicación
FROM build AS publish
RUN dotnet publish "Discoteque.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 9. Crear la imagen final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# 10. Configurar usuario no root por seguridad
RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

# 11. Comando para ejecutar la aplicación
ENTRYPOINT ["dotnet", "Discoteque.API.dll"]
```

### 3. 🎭 Docker Compose

Docker Compose es como un director de orquesta que coordina varios contenedores para que trabajen juntos. En nuestro caso, necesitamos:
- Un contenedor para la API
- Un contenedor para la base de datos
- Un contenedor para ejecutar las migraciones

```yaml
version: '3.4'

services:
  # 1. Servicio de la API
  discoteque.api:
    image: discoteque:latest
    build:
      context: .
      dockerfile: Discoteque.API/Dockerfile
    ports:
      - "5044:5044"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ConnectionStrings__DiscotequeDatabase=Host=db;Port=5432;Database=discoteque;Username=discotequeUsr;Password=localDk
    depends_on:
      migration:
        condition: service_completed_successfully
    networks:
      - discoteque_network

  # 2. Servicio de la base de datos
  db:
    image: postgres:latest
    restart: always
    environment: 
      POSTGRES_USER: discotequeUsr
      POSTGRES_PASSWORD: localDk
      POSTGRES_DB: discoteque
    ports:
      - "5432:5432"
    volumes:
      - postgres_data:/var/lib/postgresql/data
    networks:
      - discoteque_network
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U discotequeUsr -d discoteque"]
      interval: 5s
      timeout: 5s
      retries: 5

  # 3. Servicio para ejecutar migraciones
  migration:
    build:
      context: .
      dockerfile: Discoteque.API/Dockerfile
      target: build
    environment:
      - ConnectionStrings__DiscotequeDatabase=Host=db;Port=5432;Database=discoteque;Username=discotequeUsr;Password=localDk
    entrypoint: []
    command: >
      /bin/sh -c '
      dotnet tool install --global dotnet-ef &&
      export PATH="$$PATH:/root/.dotnet/tools" &&
      sleep 5 &&
      cd /src &&
      dotnet ef database update --project Discoteque.Data/Discoteque.Data.csproj --startup-project Discoteque.API/Discoteque.API.csproj --verbose
      '
    depends_on:
      db:
        condition: service_healthy
    networks:
      - discoteque_network

# 4. Configuración de red
networks:
  discoteque_network:
    driver: bridge

# 5. Volúmenes para persistencia de datos
volumes:
  postgres_data:
```

## 🎮 Jugando con Docker

### 1. 🏗️ Construyendo la Aplicación

```bash
# Construir y ejecutar todos los servicios
docker-compose up --build
```

Este comando:
- Construye las imágenes necesarias
- Crea los contenedores
- Inicia todos los servicios
- Configura la red entre contenedores
- Ejecuta las migraciones de la base de datos

### 2. 🎯 Ejecutando la Aplicación

#### 2.1 Modo Interactivo
```bash
# Ejecutar en modo interactivo (verás los logs en tiempo real)
docker-compose up
```

#### 2.2 Modo Detached (En segundo plano)
```bash
# Ejecutar en modo detached (los contenedores corren en segundo plano)
docker-compose up -d

# Ver los logs cuando lo necesites
docker-compose logs -f
```

#### 2.3 Debugging con Docker

Para hacer debugging de tu aplicación en Docker, tienes varias opciones:

1. **Usando Visual Studio Code**:
   ```json
   // .vscode/launch.json
   {
     "version": "0.2.0",
     "configurations": [
       {
         "name": "Docker .NET Launch",
         "type": "docker",
         "request": "launch",
         "preLaunchTask": "docker-run: debug",
         "netCore": {
           "appProject": "${workspaceFolder}/Discoteque.API/Discoteque.API.csproj"
         }
       }
     ]
   }
   ```

2. **Usando Visual Studio**:
   - Abre el proyecto en Visual Studio
   - Selecciona "Docker" como perfil de ejecución
   - Presiona F5 para iniciar el debugging

3. **Usando el comando attach**:
   ```bash
   # Primero, obtén el ID del contenedor
   docker ps

   # Luego, conecta el debugger al contenedor
   docker attach <container-id>
   ```

4. **Configurando el entorno de desarrollo**:
   ```bash
   # Ejecutar con variables de entorno específicas para desarrollo
   docker-compose -f docker-compose.yml -f docker-compose.dev.yml up -d
   ```

### 3. 🛑 Deteniendo la Aplicación

```bash
# Detener todos los servicios
docker-compose down

# Detener y eliminar volúmenes (útil para reiniciar desde cero)
docker-compose down -v
```

### 4. 📊 Verificando el Estado

```bash
# Ver los contenedores en ejecución
docker ps

# Ver los logs de los servicios
docker-compose logs -f

# Ver el uso de recursos
docker stats
```

### 5. 🔄 Reiniciando Servicios

```bash
# Reiniciar un servicio específico
docker-compose restart discoteque.api

# Reiniciar todos los servicios
docker-compose restart
```

## 🔍 Explorando los Conceptos Clave

### 1. 🏗️ Imágenes y Contenedores

- **Imagen**: Es como una plantilla que contiene todo lo necesario para ejecutar tu aplicación
- **Contenedor**: Es una instancia en ejecución de una imagen

### 2. 🌐 Redes

- **Red de Docker**: Permite que los contenedores se comuniquen entre sí
- **Puertos**: Permiten que el mundo exterior acceda a tu aplicación

### 3. 💾 Volúmenes

- **Volumen**: Es un espacio persistente para almacenar datos
- **Bind Mount**: Permite compartir archivos entre tu computadora y el contenedor

## 🎯 Próximos Pasos

1. **Explora más comandos de Docker**:
   ```bash
   docker images          # Ver todas las imágenes
   docker rm <container>  # Eliminar un contenedor
   docker rmi <image>     # Eliminar una imagen
   ```

2. **Aprende sobre Docker Compose**:
   - Escalado de servicios
   - Variables de entorno
   - Dependencias entre servicios

3. **Profundiza en conceptos avanzados**:
   - Multi-stage builds
   - Docker networks
   - Docker volumes

## 🎉 ¡Felicitaciones!

Has dado tus primeros pasos en el mundo de Docker y .NET. Recuerda que el aprendizaje es un proceso continuo, y cada vez que practiques, descubrirás nuevas formas de mejorar tus aplicaciones.

### 📚 Recursos Adicionales

- [Documentación oficial de Docker](https://docs.docker.com/)
- [Documentación de .NET](https://docs.microsoft.com/dotnet)
- [Docker Compose Reference](https://docs.docker.com/compose/compose-file/)

¡Sigue explorando y divirtiéndote con el desarrollo de software! 🚀
