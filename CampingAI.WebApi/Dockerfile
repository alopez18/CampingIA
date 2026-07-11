# Esta fase se usa cuando se ejecuta desde VS en modo rápido (valor predeterminado para la configuración de depuración)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# Esta fase se usa para compilar el proyecto de servicio
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["CampingAI.sln", "./"]

COPY ["CampingAI.Application.Tests/CampingAI.Application.Tests.csproj", "CampingAI.Application.Tests/"]
COPY ["CampingAI.Domain.Tests/CampingAI.Domain.Tests.csproj", "CampingAI.Domain.Tests/"]
COPY ["CampingAI.Infra.Tests/CampingAI.Infra.Tests.csproj", "CampingAI.Infra.Tests/"]

COPY ["CampingAI.Application/CampingAI.Application.csproj", "CampingAI.Application/"]
COPY ["CampingAI.Domain/CampingAI.Domain.csproj", "CampingAI.Domain/"]
COPY ["CampingAI.Infra/CampingAI.Infra.csproj", "CampingAI.Infra/"]
COPY ["CampingAI.WebApi/CampingAI.WebApi.csproj", "CampingAI.WebApi/"]


RUN dotnet restore
COPY . .
WORKDIR "/src/CampingAI.WebApi"
RUN dotnet build "CampingAI.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Esta fase se usa para publicar el proyecto de servicio que se copiará en la fase final.
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "CampingAI.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Esta fase se usa en producción o cuando se ejecuta desde VS en modo normal (valor predeterminado cuando no se usa la configuración de depuración)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "RedArbor.TT.WebApi.dll"]