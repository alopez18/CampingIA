using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CampingAI.Infra.Configuration;

public static class ContextConfiguration
{
    public const string CONNECTION_STRING_CAMPING_AI_CONFIG_NAME = "CAMPING_AI_SqlServer";
    public static void ConfigureInfra(IServiceCollection services, IConfiguration config)
    {
        //ConfigureCampinAI_EF(services, config);
        ConfigureCampingAI_Dapper(services, config);
    }

    private static void ConfigureCampingAI_Dapper(IServiceCollection services, IConfiguration config)
    {
        string? connectionString = config.GetConnectionString(CONNECTION_STRING_CAMPING_AI_CONFIG_NAME)
                                                           ?? throw new Exception($"No se ha encontrado la cadena de conexión con nombre {CONNECTION_STRING_CAMPING_AI_CONFIG_NAME}");
        services.AddScoped<Factories.Interfaces.ISqlConnectionFactory, Factories.SqlConnectionFactory>();
    }

    //private static void ConfigureCampinAI_EF(IServiceCollection services, IConfiguration config)
    //{
    //    string? connectionString = config.GetConnectionString(CONNECTION_STRING_CAMPING_AI_CONFIG_NAME)
    //                                                       ?? throw new Exception($"No se ha encontrado la cadena de conexión con nombre {CONNECTION_STRING_CAMPING_AI_CONFIG_NAME}");
    //    services.AddDbContext<Models.CAMPING_AI_DB.CAMPINGAI_TTContext>(options =>
    //    {
    //        options.UseSqlServer(connectionString);
    //    });
    //}

}
