using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace CampingAI.Infra.Configuration.Factories;

public class SqlConnectionFactory : Interfaces.ISqlConnectionFactory
{
    public const string CONNECTION_STRING_REDARBOR_CONFIG_NAME = "CAMPING_AI_SqlServer";
    private readonly string _connectionString;

    public SqlConnectionFactory(IConfiguration config)
    {
        string? connectionString = config.GetConnectionString(CONNECTION_STRING_REDARBOR_CONFIG_NAME)
                                                           ?? throw new Exception($"No se ha encontrado la cadena de conexión con nombre {CONNECTION_STRING_REDARBOR_CONFIG_NAME}");
        _connectionString = connectionString;

    }

    public IDbConnection CreateConnection()
    {
        return new SqlConnection(_connectionString);
    }

}
