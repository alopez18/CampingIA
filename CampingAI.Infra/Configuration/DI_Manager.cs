using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CampingAI.Infra.Configuration;
public static class DI_Manager {
    public static void Configure(IServiceCollection services, IConfiguration config) {
        ContextConfiguration.ConfigureInfra(services, config);
        services.AddScoped<Abstractions.IUnitOfWork, UnitOfWork>();
        RegisterRepositories(services);
        RegisterExtractors(services);
        RegisterMappers(services);
    }

    private static void RegisterMappers(IServiceCollection services) {
        services.AddScoped<Employees.Mappers.EmployeesMapper>();
    }

    private static void RegisterExtractors(IServiceCollection services) {
        services.AddSingleton<Abstractions.ModelExtractor<Models.REDARBOR_DB.T_EMPLOYEES>>();
    }

    private static void RegisterRepositories(IServiceCollection services) {
        services.AddScoped<Domain.Repositories.Employees.IEmployeesWriteRepository, Employees.EmployeesWriteRepository>();
        services.AddScoped<Domain.Repositories.Employees.IEmpoyeesReadRepository, Employees.EmployeesReadRepository>();

    }

}
