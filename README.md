# Sistema de Gestión de Biblioteca

API RESTful desarrollada con ASP.NET Core 7.0 para la gestión de libros y autores, con autenticación JWT, integración con servicios externos (REST y SOAP), y soporte para Dockerización multiplataforma.

## Características

- **Arquitectura Limpia**: Separación en capas (Dominio, Aplicación, Infraestructura, API)
- **Autenticación JWT**: Protección de endpoints con tokens Bearer
- **Validación de ISBN**: Integración con servicio SOAP externo
- **Portadas de Libros**: Obtención automática desde Open Library API
- **Normalización de Datos**: Procesamiento automático de títulos y nombres
- **Carga Masiva**: Importación de libros mediante archivos CSV
- **Documentación Swagger**: Interfaz interactiva para probar la API
- **Pruebas Unitarias**: Cobertura de servicios y controladores
- **Dockerización**: Soporte completo para Linux, Mac y Windows

## Tecnologías Utilizadas

- ASP.NET Core 7.0
- Entity Framework Core con SQLite
- JWT Bearer Authentication
- FluentValidation
- Swagger/OpenAPI
- CsvHelper
- xUnit, Moq, FluentAssertions
- Docker

## Estructura del Proyecto

```
GestionBiblioteca/
├── GestionBiblioteca.Dominio/          # Entidades y contratos
│   ├── Entidades/
│   └── Interfaces/
├── GestionBiblioteca.Aplicacion/       # Lógica de negocio
│   ├── DTOs/
│   ├── Interfaces/
│   ├── Servicios/
│   └── Validadores/
├── GestionBiblioteca.Infraestructura/  # Acceso a datos y servicios externos
│   ├── Autenticacion/
│   ├── Persistencia/
│   ├── Repositorios/
│   └── ServiciosExternos/
├── GestionBiblioteca.API/              # Capa de presentación
│   ├── Controllers/
│   └── Middleware/
├── GestionBiblioteca.Pruebas/          # Pruebas unitarias
└── Dockerfile
```

## 🚀 Instalación y Configuración

### Prerrequisitos

- .NET SDK 7.0 o superior
- Docker Desktop (para ejecutar con contenedores)

### Paso 1: Clonar y Crear la Estructura

Ejecutar los comandos iniciales proporcionados en la sección de comandos para crear la solución completa.

### Paso 2: Restaurar Dependencias

```bash
dotnet restore
```

### Paso 3: Ejecutar Migraciones

```bash
cd GestionBiblioteca.API
dotnet ef database update
```

### Paso 4: Ejecutar la Aplicación

```bash
dotnet run --project GestionBiblioteca.API
```

La aplicación estará disponible en:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`
- Swagger: `http://localhost:5000` o `https://localhost:5001`

## 🐳 Dockerización

### Construir la Imagen Docker

En el directorio raíz del proyecto (donde está el Dockerfile):

```bash
docker build -t gestion-biblioteca:latest .
```

### Ejecutar el Contenedor

#### Linux

```bash
docker run -d -p 8080:80 --name biblioteca-api gestion-biblioteca:latest
```

Acceder a: `http://localhost:8080`

#### macOS

```bash
docker run -d -p 8080:80 --name biblioteca-api gestion-biblioteca:latest
```

Acceder a: `http://localhost:8080`

#### Windows 11

**PowerShell:**
```powershell
docker run -d -p 8080:80 --name biblioteca-api gestion-biblioteca:latest
```

**CMD:**
```cmd
docker run -d -p 8080:80 --name biblioteca-api gestion-biblioteca:latest
```

Acceder a: `http://localhost:8080`

### Verificar que el Contenedor está Corriendo

```bash
docker ps
```

### Ver Logs del Contenedor

```bash
docker logs biblioteca-api
```

### Detener el Contenedor

```bash
docker stop biblioteca-api
```

### Eliminar el Contenedor

```bash
docker rm biblioteca-api
```

## Autenticación

### Obtener Token JWT

**Endpoint:** `GET /api/login`

**Parámetros de consulta:**
- `nombreUsuario`: Nombre de usuario
- `contrasenia`: Contraseña

**Usuarios Precargados:**
- Usuario: `admin` / Contraseña: `password123`
- Usuario: `usuario` / Contraseña: `12345`

**Ejemplo con cURL:**
```bash
curl -X GET "http://localhost:8080/api/login?nombreUsuario=admin&contrasenia=password123"
```

**Respuesta:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiracion": "2026-01-17T10:30:00Z"
}
```

### Usar el Token

Incluir el token en el encabezado `Authorization` de todas las peticiones:

```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

## Endpoints de la API

### Autores

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/api/authors` | Obtener todos los autores (paginado) |
| POST | `/api/authors` | Crear un nuevo autor |
| PATCH | `/api/authors/{id}` | Actualizar un autor parcialmente |
| DELETE | `/api/authors/{id}` | Eliminar un autor por ID |

### Libros

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/api/books` | Obtener todos los libros (paginado, filtrable) |
| GET | `/api/books/{id}` | Obtener un libro por ID |
| POST | `/api/books` | Crear un nuevo libro |
| PATCH | `/api/books/{id}` | Actualizar un libro parcialmente |
| DELETE | `/api/books/{id}` | Eliminar un libro por ID |
| GET | `/api/books/validation/{isbn}` | Validar un ISBN |
| POST | `/api/books/masive` | Crear libros de forma masiva (CSV) |

## 📝 Ejemplos de Uso

### Crear un Autor

```bash
curl -X POST "http://localhost:8080/api/authors" \
  -H "Authorization: Bearer {token}" \
  -H "Content-Type: application/json" \
  -d '{
    "nombre": "Gabriel García Márquez"
  }'
```

### Crear un Libro

```bash
curl -X POST "http://localhost:8080/api/books" \
  -H "Authorization: Bearer {token}" \
  -H "Content-Type: application/json" \
  -d '{
    "isbn": "9780307474728",
    "titulo": "Cien años de soledad",
    "anioPublicacion": 1967,
    "autorId": "guid-del-autor"
  }'
```

### Buscar Libros

```bash
curl -X GET "http://localhost:8080/api/books?pagina=1&tamanio=10&titulo=soledad" \
  -H "Authorization: Bearer {token}"
```

### Validar ISBN

```bash
curl -X GET "http://localhost:8080/api/books/validation/9780307474728" \
  -H "Authorization: Bearer {token}"
```

### Carga Masiva de Libros (CSV)

**Formato del archivo CSV (`libros.csv`):**
```csv
isbn,titulo,anioPublicacion,nombreAutor
9780307474728,Cien años de soledad,1967,Gabriel García Márquez
9780142437339,Don Quijote de la Mancha,1605,Miguel de Cervantes
```

**Enviar archivo:**
```bash
curl -X POST "http://localhost:8080/api/books/masive" \
  -H "Authorization: Bearer {token}" \
  -F "archivo=@libros.csv"
```

## Ejecutar Pruebas

```bash
dotnet test
```

## Configuración

El archivo `appsettings.json` contiene la configuración de la aplicación:

```json
{
  "Jwt": {
    "Key": "ClaveSecretaSuperSeguraParaJWT2024!@#$%^&*()",
    "Issuer": "GestionBiblioteca",
    "Audience": "GestionBibliotecaAPI"
  },
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=biblioteca.db"
  }
}
```

## Requisitos Funcionales Implementados

✅ Arquitectura limpia con separación de capas  
✅ Entity Framework Core con SQLite  
✅ Modelos Book y Author con relación CASCADE  
✅ Autenticación JWT con expiración de 1 hora  
✅ Usuarios precargados en la base de datos  
✅ Protección de endpoints con [Authorize]  
✅ CRUD completo de libros y autores  
✅ Paginación y búsqueda en listados  
✅ Integración con Open Library API (REST)  
✅ Integración con servicio de validación ISBN (SOAP)  
✅ Normalización de textos (mayúsculas, sin números, sin tildes)  
✅ Validación con FluentValidation  
✅ Carga masiva desde CSV  
✅ Documentación con Swagger/OpenAPI  
✅ Manejo centralizado de excepciones con ProblemDetails  
✅ Pruebas unitarias de servicios y controladores  
✅ Dockerfile multi-etapa optimizado  
✅ Soporte para Linux, Mac y Windows 11

## Normalización de Datos

El sistema aplica automáticamente las siguientes normalizaciones a títulos de libros y nombres de autores:

1. Conversión a MAYÚSCULAS
2. Eliminación de números
3. Reemplazo de caracteres especiales (á→A, ñ→N, etc.)
4. Reemplazo de espacios múltiples por uno solo

**Ejemplo:**
- Entrada: `"Programación 2024: El año de la IA"` 
- Salida: `"PROGRAMACION EL ANO DE LA IA"`

## Base de Datos

La aplicación utiliza SQLite con las siguientes tablas:

- **Usuarios**: Almacena credenciales (precargadas)
- **Autores**: Información de autores
- **Libros**: Información de libros con relación a Autores

La relación entre Libro y Autor está configurada con **DELETE CASCADE**: al eliminar un autor, se eliminan automáticamente todos sus libros.

## 🐛 Solución de Problemas

### Error: "No se puede conectar al servicio SOAP"

El servicio de validación de ISBN tiene un fallback que valida el formato básico del ISBN si el servicio externo no está disponible.

### Error: "No se encuentra la portada"

Si Open Library no tiene la portada del libro, el campo `urlPortada` quedará vacío. Esto no impide la creación del libro.

### Error: "Puerto 8080 ya está en uso"

Cambiar el puerto en el comando docker run:
```bash
docker run -d -p 8081:80 --name biblioteca-api gestion-biblioteca:latest
```

---

**Nota**: Este README contiene toda la información necesaria para construir, ejecutar y probar la aplicación en cualquier plataforma compatible.
