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

/// Configuración de DbContext con SQLite en memoria
builder.Services.AddDbContext<BibliotecaDbContext>(options =>
    options.UseSqlite("Data Source=biblioteca.db"));

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
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<BibliotecaDbContext>();
    context.Database.EnsureCreated();
}

// Middleware de manejo de excepciones
app.UseMiddleware<ManejadorExcepcionesMiddleware>();

// Configuración del pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Gestión Biblioteca v1");
        c.RoutePrefix = string.Empty; // Swagger en la raíz
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

// Hacer la clase Program parcial para pruebas
public partial class Program { }
