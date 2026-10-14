# Etapa 1: Compilação e publicação
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY ["TaskManagement.csproj", "./"]

RUN dotnet restore "TaskManagement.csproj"

COPY . .

RUN dotnet publish "TaskManagement.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore

# Etapa 2: Execução da API
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080

ENTRYPOINT ["dotnet", "TaskManagement.dll"]
