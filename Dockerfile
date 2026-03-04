# Eerste fase: applicatie bouwen
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Alle projectbestanden kopiëren naar de container
COPY . .

# NuGet packages downloaden
RUN dotnet restore

# Applicatie builden en publiceren
RUN dotnet publish -c Release -o /app/publish


# Tweede fase: alleen de runtime gebruiken
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# De gepubliceerde app uit de build fase kopiëren
COPY --from=build /app/publish .

# Instellen op welke poort de app draait
ENV ASPNETCORE_URLS=http://+:8080

# Deze poort openstellen voor de container
EXPOSE 8080

# Start de applicatie wanneer de container start
ENTRYPOINT ["dotnet", "PhotoBooth.dll"]