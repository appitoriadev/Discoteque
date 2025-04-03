# Implementación de JWT, Autenticación y Autorización en Discoteque API

## Índice
1. [Configuración Inicial](#configuración-inicial)
2. [Configuración de JWT](#configuración-de-jwt)
3. [Configuración de Swagger](#configuración-de-swagger)
4. [Implementación de Servicios](#implementación-de-servicios)
5. [Protección de Endpoints](#protección-de-endpoints)
6. [Consideraciones de Seguridad](#consideraciones-de-seguridad)
7. [Uso en Swagger](#uso-en-swagger)
8. [Pruebas](#pruebas)

## Configuración Inicial

### 1.1 Estructura del Proyecto
La solución Discoteque está organizada en las siguientes capas:
- **Discoteque.API**: Capa de presentación y controladores
- **Discoteque.Business**: Capa de lógica de negocio
- **Discoteque.Data**: Capa de acceso a datos
- **Discoteque.Tests**: Pruebas unitarias y de integración

### 1.2 Dependencias Necesarias
Asegúrate de tener los siguientes paquetes NuGet instalados:
```xml
<PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="9.0.0" />
<PackageReference Include="Microsoft.AspNetCore.OpenApi" Version="9.0.0" />
<PackageReference Include="Swashbuckle.AspNetCore" Version="6.5.0" />
```

## Configuración de JWT

### 2.1 Configuración en appsettings.json
```json
{
  "Jwt": {
    "Key": "BVTb0zz9kwkdeWe2mo3H8+DB1ARlDSyhI+xgaKXaj6w=",
    "Issuer": "YourLocalHost",
    "Audience": "YourLocalHost",
    "ExpirationMinutes": 30,
    "RefreshTokenExpirationDays": 7
  }
}
```

### 2.2 Configuración en Program.cs
```csharp
builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options => {
    options.RequireHttpsMetadata = false; // Cambiar a true en producción
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
    };
});
```

## Configuración de Swagger

### 3.1 Configuración de Swagger con JWT
```csharp
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Discoteque API", Version = "v1" });
    
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' followed by a space and the token."
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});
```

## Implementación de Servicios

### 4.1 Servicios de Autenticación
La solución incluye los siguientes servicios:
```csharp
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
```

### 4.2 Configuración del Pipeline HTTP
```csharp
app.UseAuthentication();
app.UseAuthorization();
```

## Protección de Endpoints

### 5.1 Ejemplo de Controlador Protegido
```csharp
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class YourController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        // Endpoint protegido
    }
}
```

## Consideraciones de Seguridad

1. **Configuración de Producción**
   - Cambiar `RequireHttpsMetadata` a `true` en producción
   - Usar una clave JWT segura y única
   - Almacenar la clave JWT en variables de entorno o secretos

2. **Mejores Prácticas**
   - Implementar refresh tokens para mejor seguridad
   - Configurar CORS si es necesario
   - Validar todos los inputs
   - Implementar rate limiting
   - Usar HTTPS en producción

## Uso en Swagger

1. **Obtener Token**
   - Usar el endpoint de login para obtener el token JWT

2. **Autorizar en Swagger**
   - Hacer clic en el botón "Authorize"
   - Ingresar el token en el formato: `Bearer <token>`
   - Los endpoints protegidos serán accesibles

## Pruebas

1. **Verificación de Endpoints Protegidos**
   - Probar acceso sin token (debe fallar)
   - Probar acceso con token válido
   - Probar acceso con token expirado

2. **Flujo Completo**
   - Login
   - Obtención de token
   - Acceso a recursos protegidos
   - Verificación de expiración
   - Manejo de tokens inválidos

## Conclusión

Esta implementación proporciona una base sólida para la autenticación y autorización en la API de Discoteque, con soporte completo para Swagger y siguiendo las mejores prácticas de seguridad. La documentación y el código están diseñados para facilitar el onboarding de nuevos desarrolladores y mantener un alto nivel de seguridad en la aplicación. 