using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using CampingAI.Application.Abstractions;
using CampingAI.Application.Abstractions.Command;
using CampingAI.Application.Abstractions.Query;
using CampingAI.Application.Queries.Employee.GetEmployeeById;
using CampingAI.Application.Queries.Employees.GetEmployees;
using CampingAI.Application.Shared.DTOs;
using CampingAI.Application.Shared.Mappers;

namespace CampingAI.Application.Configuration;
public static class DI_Manager {

    public static void Configure(IServiceCollection services, IConfiguration config) {
        services.AddScoped<IMediator, Mediator>();
        RegisterCommands(services);
        RegisterQueries(services);
        RegisterMappers(services);
        RegisterServices(services);


        Infra.Configuration.DI_Manager.Configure(services, config);
    }

    private static void RegisterServices(IServiceCollection services) {
        services.AddScoped<Services.PasswordHashingService.Interfaces.IPasswordHashingService, Services.PasswordHashingService.PasswordHashingService>();
        services.AddScoped<IValidator<Commands.Employee.CreateEmployee.CreateEmployeeCommand>, Commands.Employee.CreateEmployee.CreateEmployeeCommandValidator>();
    }

    private static void RegisterMappers(IServiceCollection services) {
        services.AddScoped<GetEmployeeByIdItemDtoMapper>();
    }

    private static void RegisterQueries(IServiceCollection services) {
        services.AddScoped<IQueryHandler<GetEmployeeByIdQuery, GetEmployeeByIdItemDto>, GetEmployeeByIdQueryHandler>();
        services.AddScoped<IQueryHandler<GetEmployeesQuery, IEnumerable<GetEmployeeByIdItemDto>>, GetEmployeesQueryHandler>();
    }

    private static void RegisterCommands(IServiceCollection services) {
        services.AddScoped<ICommandHandler<Commands.Employee.CreateEmployee.CreateEmployeeCommand, Domain.Entities.Employee>, Commands.Employee.CreateEmployee.CreateEmployeeCommandHandler>();
        services.AddScoped<ICommandHandler<Commands.Employee.UpdateEmployee.UpdateEmployeeCommand, Domain.Entities.Employee>, Commands.Employee.UpdateEmployee.UpdateEmployeeeCommandHandler>();
        services.AddScoped<ICommandHandler<Commands.Employee.DeleteEmployee.DeleteEmployeeCommand>, Commands.Employee.DeleteEmployee.DeleteEmployeeCommandHandler>();
    }


}
