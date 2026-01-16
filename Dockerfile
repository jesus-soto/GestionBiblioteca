FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

COPY ["GestionBiblioteca.API/GestionBiblioteca.API.csproj", "GestionBiblioteca.API/"]
COPY ["GestionBiblioteca.Aplicacion/GestionBiblioteca.Aplicacion.csproj", "GestionBiblioteca.Aplicacion/"]
COPY ["GestionBiblioteca.Dominio/GestionBiblioteca.Dominio.csproj", "GestionBiblioteca.Dominio/"]
COPY ["GestionBiblioteca.Infraestructura/GestionBiblioteca.Infraestructura.csproj", "GestionBiblioteca.Infraestructura/"]

RUN dotnet restore "GestionBiblioteca.API/GestionBiblioteca.API.csproj"

COPY . .
WORKDIR "/src/GestionBiblioteca.API"
RUN dotnet build "GestionBiblioteca.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GestionBiblioteca.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
WORKDIR /app
EXPOSE 80
EXPOSE 443

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "GestionBiblioteca.API.dll"]