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
        services.AddScoped<IValidator<Commands.User.RegisterUser.RegisterUserCommand>, Commands.User.RegisterUser.RegisterUserCommandValidator>();
        services.AddScoped<IValidator<Commands.User.LoginUser.LoginUserCommand>, Commands.User.LoginUser.LoginUserCommandValidator>();
        services.AddScoped<IValidator<Commands.User.UpdateUser.UpdateUserCommand>, Commands.User.UpdateUser.UpdateUserCommandValidator>();
        services.AddScoped<IValidator<Commands.Camping.CreateCamping.CreateCampingCommand>, Commands.Camping.CreateCamping.CreateCampingCommandValidator>();
        services.AddScoped<IValidator<Commands.Camping.UpdateCamping.UpdateCampingCommand>, Commands.Camping.UpdateCamping.UpdateCampingCommandValidator>();
        services.AddScoped<IValidator<Commands.Camping.DeleteCamping.DeleteCampingCommand>, Commands.Camping.DeleteCamping.DeleteCampingCommandValidator>();
    }

    private static void RegisterMappers(IServiceCollection services) {
        services.AddScoped<GetEmployeeByIdItemDtoMapper>();
    }

    private static void RegisterQueries(IServiceCollection services) {
        services.AddScoped<IQueryHandler<GetEmployeeByIdQuery, GetEmployeeByIdItemDto>, GetEmployeeByIdQueryHandler>();
        services.AddScoped<IQueryHandler<GetEmployeesQuery, IEnumerable<GetEmployeeByIdItemDto>>, GetEmployeesQueryHandler>();
        services.AddScoped<IQueryHandler<Queries.User.GetCurrentUser.GetCurrentUserQuery, Domain.Entities.User>, Queries.User.GetCurrentUser.GetCurrentUserQueryHandler>();
        services.AddScoped<IQueryHandler<Queries.User.GetUserById.GetUserByIdQuery, Domain.Entities.User>, Queries.User.GetUserById.GetUserByIdQueryHandler>();
        services.AddScoped<IQueryHandler<Queries.Camping.GetCampings.GetCampingsQuery, Queries.Camping.GetCampings.GetCampingsResult>, Queries.Camping.GetCampings.GetCampingsQueryHandler>();
        services.AddScoped<IQueryHandler<Queries.Camping.GetCampingById.GetCampingByIdQuery, Domain.Entities.Camping>, Queries.Camping.GetCampingById.GetCampingByIdQueryHandler>();
    }

    private static void RegisterCommands(IServiceCollection services) {
        services.AddScoped<ICommandHandler<Commands.Employee.CreateEmployee.CreateEmployeeCommand, Domain.Entities.Employee>, Commands.Employee.CreateEmployee.CreateEmployeeCommandHandler>();
        services.AddScoped<ICommandHandler<Commands.Employee.UpdateEmployee.UpdateEmployeeCommand, Domain.Entities.Employee>, Commands.Employee.UpdateEmployee.UpdateEmployeeeCommandHandler>();
        services.AddScoped<ICommandHandler<Commands.Employee.DeleteEmployee.DeleteEmployeeCommand>, Commands.Employee.DeleteEmployee.DeleteEmployeeCommandHandler>();
        services.AddScoped<ICommandHandler<Commands.User.RegisterUser.RegisterUserCommand, Domain.Entities.User>, Commands.User.RegisterUser.RegisterUserCommandHandler>();
        services.AddScoped<ICommandHandler<Commands.User.LoginUser.LoginUserCommand, string>, Commands.User.LoginUser.LoginUserCommandHandler>();
        services.AddScoped<ICommandHandler<Commands.User.UpdateUser.UpdateUserCommand, Domain.Entities.User>, Commands.User.UpdateUser.UpdateUserCommandHandler>();
        services.AddScoped<ICommandHandler<Commands.Camping.CreateCamping.CreateCampingCommand, Domain.Entities.Camping>, Commands.Camping.CreateCamping.CreateCampingCommandHandler>();
        services.AddScoped<ICommandHandler<Commands.Camping.UpdateCamping.UpdateCampingCommand, Domain.Entities.Camping>, Commands.Camping.UpdateCamping.UpdateCampingCommandHandler>();
        services.AddScoped<ICommandHandler<Commands.Camping.DeleteCamping.DeleteCampingCommand>, Commands.Camping.DeleteCamping.DeleteCampingCommandHandler>();
    }


}
