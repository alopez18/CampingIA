# Peculiaridades de la capa de infraestructura

Para generar el modelo debes ejecutar la siguiente instrucción en la consola del administrador de paquetes:

### Para la base de datos de RVSA
`Scaffold-DbContext "Server=localhos,1433;Database=CAMPINGAI_TT;User=sa;Password=Pwd_12345!;TrustServerCertificate=True;Language=Spanish;" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models\CAMPING_AI_DB -NoPluralize -UseDatabasenames -Tables "[dbo].[T_EMPLOYEES]"  -NoOnConfiguring -Force`