using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using FluentValidation;
using FluentValidation.AspNetCore;
using GestionBiblioteca.Aplicacion.Interfaces;
using GestionBiblioteca.Aplicacion.Servicios;
using GestionBiblioteca.Aplicacion.Validadores;
using GestionBiblioteca.Dominio.Interfaces;
using GestionBiblioteca.Infraestructura.Autenticacion;
using GestionBiblioteca.Infraestructura.Persistencia;
using GestionBiblioteca.Infraestructura.Repositorios;
using GestionBiblioteca.Infraestructura.ServiciosExternos;
using GestionBiblioteca.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Configurar para escuchar en todas las interfaces
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(80);
});

// Configuración de DbContext con SQLite
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Data Source=/app/data/biblioteca.db";

builder.Services.AddDbContext<BibliotecaDbContext>(options =>
    options.UseSqlite(connectionString));

// Repositorios
builder.Services.AddScoped<IRepositorioLibro, RepositorioLibro>();
builder.Services.AddScoped<IRepositorioAutor, RepositorioAutor>();
builder.Services.AddScoped<IRepositorioUsuario, RepositorioUsuario>();

// Servicios de Aplicación
builder.Services.AddScoped<IServicioLibro, ServicioLibro>();
builder.Services.AddScoped<IServicioAutor, ServicioAutor>();
builder.Services.AddScoped<IServicioAutenticacion, ServicioAutenticacion>();
builder.Services.AddScoped<IServicioNormalizacion, ServicioNormalizacion>();

// Servicios Externos
builder.Services.AddScoped<IServicioValidacionIsbn, ServicioValidacionIsbn>();
builder.Services.AddScoped<IServicioPortadaLibro, ServicioPortadaLibro>();

// HttpClient
builder.Services.AddHttpClient();

// Validadores FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CrearLibroValidador>();

// Configuración de JWT
var jwtKey = builder.Configuration["Jwt:Key"] ?? "ClaveSecretaSuperSeguraParaJWT2024!@#$%^&*()";
var key = Encoding.ASCII.GetBytes(jwtKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "GestionBiblioteca",
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"] ?? "GestionBibliotecaAPI",
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// Controladores
builder.Services.AddControllers();

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API de Gestión de Biblioteca",
        Version = "v1",
        Description = "API RESTful para gestión de libros y autores con autenticación JWT",
        Contact = new OpenApiContact
        {
            Name = "Gestión Biblioteca",
            Email = "contacto@gestionbiblioteca.com"
        }
    });

    // Configuración de seguridad JWT en Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Autenticación JWT usando el esquema Bearer. Ingrese 'Bearer' [espacio] y luego su token.\r\n\r\nEjemplo: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

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
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Crear base de datos y aplicar migraciones
try
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<BibliotecaDbContext>();
        context.Database.EnsureCreated();
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Error creando la base de datos: {ex.Message}");
}

// Middleware de manejo de excepciones
app.UseMiddleware<ManejadorExcepcionesMiddleware>();

// Health check endpoint (público)
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
    .WithName("Health")
    .WithOpenApi()
    .AllowAnonymous();

// Configuración del pipeline HTTP
// Habilitar Swagger en todos los ambientes para debugging
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Gestión Biblioteca v1");
    c.RoutePrefix = string.Empty; // Swagger en la raíz
});

// No redirigir a HTTPS en Docker (solo HTTP disponible)
if (!app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

// Hacer la clase Program parcial para pruebas
public partial class Program { }