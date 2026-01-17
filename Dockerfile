# Etapa de construcción
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar archivos de solución y proyectos
COPY ["GestionBiblioteca.API/GestionBiblioteca.API.csproj", "GestionBiblioteca.API/"]
COPY ["GestionBiblioteca.Aplicacion/GestionBiblioteca.Aplicacion.csproj", "GestionBiblioteca.Aplicacion/"]
COPY ["GestionBiblioteca.Dominio/GestionBiblioteca.Dominio.csproj", "GestionBiblioteca.Dominio/"]
COPY ["GestionBiblioteca.Infraestructura/GestionBiblioteca.Infraestructura.csproj", "GestionBiblioteca.Infraestructura/"]

# Restaurar dependencias
RUN dotnet restore "GestionBiblioteca.API/GestionBiblioteca.API.csproj"

# Copiar el resto del código
COPY . .

# Compilar
WORKDIR "/src/GestionBiblioteca.API"
RUN dotnet build "GestionBiblioteca.API.csproj" -c Release -o /app/build

# Publicar la aplicación
FROM build AS publish
RUN dotnet publish "GestionBiblioteca.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Etapa final - Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Crear directorio para la base de datos con permisos
RUN mkdir -p /app/data && chmod 777 /app/data

# Configurar variables de entorno
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production
ENV DOTNET_RUNNING_IN_CONTAINER=true

EXPOSE 80

# Copiar archivos publicados
COPY --from=publish /app/publish .

# Crear usuario no-root para seguridad
RUN groupadd -r appuser && useradd -r -g appuser appuser
RUN chown -R appuser:appuser /app
USER appuser

# Punto de entrada
ENTRYPOINT ["dotnet", "GestionBiblioteca.API.dll"]