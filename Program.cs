using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using fut7Manager.Data;
using Microsoft.OpenApi.Models;

//using fut7Manager.Services;

/*
 Request pipeline
 Request HTTP
     ▼
Authentication middleware (verifica JWT)
     ▼
Authorization middleware (verifica [Authorize])
     ▼
Exception middleware (captura errores)
     ▼
Controller
     ▼
UserService
     ▼
Entity Framework
     ▼
SQL Server
 */

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

//builder.Services.AddAutoMapper(typeof(Program)); // registra AutoMapper

builder.Services.AddEndpointsApiExplorer(); // Permite que Swagger descubra los endpoints automáticamente

builder.Services.AddSwaggerGen(options => { // Configuración de Swagger (documentación de la API)
    options.SwaggerDoc("v1", new() { Title = "fut7Manager", Version = "v1" }); // Define un documento Swagger versión 1

    // Define el esquema de seguridad JWT para Swagger. Esto permite que aparezca el botón "Authorize"
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
        Name = "Authorization", // Nombre del header HTTP
        Type = SecuritySchemeType.Http,  // Tipo de autenticación HTTP
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header, // El token se envía en el header
        Description = "Enter JWT token" // Texto que aparece en Swagger
    });


    // Indica a Swagger que los endpoints pueden requerir JWT. Esto hace que Swagger agregue automáticamente el token
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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

var key = builder.Configuration["Jwt:Key"] // Clave secreta usada para firmar los JWT
          ?? throw new Exception("JWT Key not configured");

// Configuración del sistema de autenticación
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => {
    // Parámetros de validación del token
    options.TokenValidationParameters = new TokenValidationParameters {
        ValidateIssuer = false, // No valida el issuer (quién emitió el token)
        ValidateAudience = false, // No valida audiencia (para quién es el token)
        ValidateLifetime = true, // Valida que el token no haya expirado
        ValidateIssuerSigningKey = true, // Verifica que la firma del token sea válida
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(key!)) // Clave usada para validar la firma del JWT
    };
});

//builder.Services.AddOpenApi();

var app = builder.Build(); // Construye la aplicación con toda la configuración anterior

app.UseAuthentication(); // Middleware que valida el JWT en cada request

app.UseAuthorization(); // Middleware que aplica las reglas de autorización ([Authorize])

app.UseMiddleware<fut7Manager.Middleware.ExceptionMiddleware>(); // Middleware personalizado para manejar excepciones globalmente

app.UseSwagger(); // Activa Swagger

app.UseSwaggerUI(); // Interfaz web de Swagger

app.MapControllers(); // Mapea automáticamente los controllers como endpoints HTTP

//app.UseHttpsRedirection();

app.Run(); // Inicia el servidor web