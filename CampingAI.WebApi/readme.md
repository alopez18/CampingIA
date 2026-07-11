# Prueba técnica para RedArbor

Este proyecto es una prueba técnica desarrollada para RedArbor, que incluye una API web construida con ASP.NET Core.


## Requisitos
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://www.docker.com/get-started)

## Instalación y configuración

### Desarrollo local
1. Clona este repositorio:
   ```bash
   git clone https://github.com/alopez18/redarbor_tt.git
1. Navegamos al directorio del proyecto donde hayas clonado el repositorio:
   ```bash
   cd redarbor_tt
   ```
   
1. Levantamos SQL Server en un contenedor de Docker (si no tienes SQL Server instalado localmente):
   ```bash
   docker run --name=sql-redarbor-tt --hostname=sql-redarbor-tt --user=mssql --env=MSSQL_PID=Developer --env=ACCEPT_EULA=Y --env=MSSQL_SA_PASSWORD=Pwd_12345! --volume=sql_data:/var/opt/mssql --network=bridge -p 11433:1433 --restart=no -d mcr.microsoft.com/mssql/server:2022-latest
   ```
1. Conectamos mediante un cliente de sql server y ejecutamos el script de inicio que se encutra en:
   ```bash
   ~/CampingAI.Infra\0.CreateDatabase.sql
   ```
1. Creamos red de docker:
   ```bash
	docker network create redarbor-network-dev
	```
	```bash
	docker network connect redarbor-network-dev sql-redarbor-tt
	```
	```bash
	docker network connect redarbor-network-dev CampingAI.WebApi
	```
	```bash
	docker network inspect redarbor-network-dev
	```
1. Iniciamos el perfil de inicio "Container (Dockerfile)"

### Producción con Docker
1. Accedemos a la carpeta del proyecto:
   ```bash
   cd CampingAI.WebApi
   ```
1. Ejecutamos el comando de docker compose para levantar los servicios:
   ```bash
   docker-compose -f docker-compose.yml up --build -d
   ```

### Para comprobar que funciona
1. abrimos la web en el navegador:
   ```
   http://localhost:5000/
   ```

