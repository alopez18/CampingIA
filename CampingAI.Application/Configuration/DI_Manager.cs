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
        services.AddScoped<IValidator<Commands.User.RegisterManager.RegisterManagerCommand>, Commands.User.RegisterManager.RegisterManagerCommandValidator>();
        services.AddScoped<IValidator<Commands.Camping.CreateCamping.CreateCampingCommand>, Commands.Camping.CreateCamping.CreateCampingCommandValidator>();
        services.AddScoped<IValidator<Commands.Camping.UpdateCamping.UpdateCampingCommand>, Commands.Camping.UpdateCamping.UpdateCampingCommandValidator>();
        services.AddScoped<IValidator<Commands.Camping.DeleteCamping.DeleteCampingCommand>, Commands.Camping.DeleteCamping.DeleteCampingCommandValidator>();
        services.AddScoped<IValidator<Queries.Camping.SearchCampings.SearchCampingsQuery>, Queries.Camping.SearchCampings.SearchCampingsQueryValidator>();
        services.AddScoped<IValidator<Commands.Favorite.AddFavorite.AddFavoriteCommand>, Commands.Favorite.AddFavorite.AddFavoriteCommandValidator>();
        services.AddScoped<IValidator<Commands.Favorite.RemoveFavorite.RemoveFavoriteCommand>, Commands.Favorite.RemoveFavorite.RemoveFavoriteCommandValidator>();
        services.AddScoped<IValidator<Commands.Reservation.CreateReservation.CreateReservationCommand>, Commands.Reservation.CreateReservation.CreateReservationCommandValidator>();
        services.AddScoped<IValidator<Commands.Reservation.CancelReservation.CancelReservationCommand>, Commands.Reservation.CancelReservation.CancelReservationCommandValidator>();
    }

    private static void RegisterMappers(IServiceCollection services) {
        services.AddScoped<GetEmployeeByIdItemDtoMapper>();
    }

    private static void RegisterQueries(IServiceCollection services) {
        services.AddScoped<IQueryHandler<GetEmployeeByIdQuery, GetEmployeeByIdItemDto>, GetEmployeeByIdQueryHandler>();
        services.AddScoped<IQueryHandler<GetEmployeesQuery, IEnumerable<GetEmployeeByIdItemDto>>, GetEmployeesQueryHandler>();
        services.AddScoped<IQueryHandler<Queries.User.GetCurrentUser.GetCurrentUserQuery, Domain.Entities.User>, Queries.User.GetCurrentUser.GetCurrentUserQueryHandler>();
        services.AddScoped<IQueryHandler<Queries.User.GetUserById.GetUserByIdQuery, Domain.Entities.User>, Queries.User.GetUserById.GetUserByIdQueryHandler>();
        services.AddScoped<IQueryHandler<Queries.User.GetPendingManagers.GetPendingManagersQuery, IEnumerable<Domain.Entities.User>>, Queries.User.GetPendingManagers.GetPendingManagersQueryHandler>();
        services.AddScoped<IQueryHandler<Queries.Camping.GetCampings.GetCampingsQuery, Queries.Camping.GetCampings.GetCampingsResult>, Queries.Camping.GetCampings.GetCampingsQueryHandler>();
        services.AddScoped<IQueryHandler<Queries.Camping.GetCampingsByOwner.GetCampingsByOwnerQuery, IEnumerable<Domain.Entities.Camping>>, Queries.Camping.GetCampingsByOwner.GetCampingsByOwnerQueryHandler>();
        services.AddScoped<IQueryHandler<Queries.Camping.GetCampingById.GetCampingByIdQuery, Domain.Entities.Camping>, Queries.Camping.GetCampingById.GetCampingByIdQueryHandler>();
        services.AddScoped<IQueryHandler<Queries.Camping.SearchCampings.SearchCampingsQuery, Queries.Camping.SearchCampings.SearchCampingsResult>, Queries.Camping.SearchCampings.SearchCampingsQueryHandler>();
        services.AddScoped<IQueryHandler<Queries.Location.GetCountries.GetCountriesQuery, Queries.Location.GetCountries.GetCountriesResult>, Queries.Location.GetCountries.GetCountriesQueryHandler>();
        services.AddScoped<IQueryHandler<Queries.Location.GetProvinces.GetProvincesQuery, Queries.Location.GetProvinces.GetProvincesResult>, Queries.Location.GetProvinces.GetProvincesQueryHandler>();
        services.AddScoped<IQueryHandler<Queries.Category.GetCategories.GetCategoriesQuery, Queries.Category.GetCategories.GetCategoriesResult>, Queries.Category.GetCategories.GetCategoriesQueryHandler>();
        services.AddScoped<IQueryHandler<Queries.Facility.GetFacilities.GetFacilitiesQuery, Queries.Facility.GetFacilities.GetFacilitiesResult>, Queries.Facility.GetFacilities.GetFacilitiesQueryHandler>();
        services.AddScoped<IQueryHandler<Queries.Favorite.GetFavorites.GetFavoritesQuery, IEnumerable<Domain.Entities.Favorite>>, Queries.Favorite.GetFavorites.GetFavoritesQueryHandler>();
        services.AddScoped<IQueryHandler<Queries.Reservation.GetReservationById.GetReservationByIdQuery, Domain.Entities.Reservation>, Queries.Reservation.GetReservationById.GetReservationByIdQueryHandler>();
        services.AddScoped<IQueryHandler<Queries.Reservation.GetUserReservations.GetUserReservationsQuery, IEnumerable<Domain.Entities.Reservation>>, Queries.Reservation.GetUserReservations.GetUserReservationsQueryHandler>();
    }

    private static void RegisterCommands(IServiceCollection services) {
        services.AddScoped<ICommandHandler<Commands.Employee.CreateEmployee.CreateEmployeeCommand, Domain.Entities.Employee>, Commands.Employee.CreateEmployee.CreateEmployeeCommandHandler>();
        services.AddScoped<ICommandHandler<Commands.Employee.UpdateEmployee.UpdateEmployeeCommand, Domain.Entities.Employee>, Commands.Employee.UpdateEmployee.UpdateEmployeeeCommandHandler>();
        services.AddScoped<ICommandHandler<Commands.Employee.DeleteEmployee.DeleteEmployeeCommand>, Commands.Employee.DeleteEmployee.DeleteEmployeeCommandHandler>();
        services.AddScoped<ICommandHandler<Commands.User.RegisterUser.RegisterUserCommand, Domain.Entities.User>, Commands.User.RegisterUser.RegisterUserCommandHandler>();
        services.AddScoped<ICommandHandler<Commands.User.LoginUser.LoginUserCommand, string>, Commands.User.LoginUser.LoginUserCommandHandler>();
        services.AddScoped<ICommandHandler<Commands.User.GoogleLoginUser.GoogleLoginUserCommand, string>, Commands.User.GoogleLoginUser.GoogleLoginUserCommandHandler>();
        services.AddScoped<ICommandHandler<Commands.User.UpdateUser.UpdateUserCommand, Domain.Entities.User>, Commands.User.UpdateUser.UpdateUserCommandHandler>();
        services.AddScoped<ICommandHandler<Commands.User.RequestManagerRole.RequestManagerRoleCommand, Domain.Entities.User>, Commands.User.RequestManagerRole.RequestManagerRoleCommandHandler>();
        services.AddScoped<ICommandHandler<Commands.User.GoogleRegisterManager.GoogleRegisterManagerCommand, Domain.Entities.User>, Commands.User.GoogleRegisterManager.GoogleRegisterManagerCommandHandler>();
        services.AddScoped<ICommandHandler<Commands.User.RegisterManager.RegisterManagerCommand, Domain.Entities.User>, Commands.User.RegisterManager.RegisterManagerCommandHandler>();
        services.AddScoped<ICommandHandler<Commands.User.RequestManagerRole.RequestManagerRoleCommand, Domain.Entities.User>, Commands.User.RequestManagerRole.RequestManagerRoleCommandHandler>();
        services.AddScoped<ICommandHandler<Commands.User.ApproveManager.ApproveManagerCommand, Domain.Entities.User>, Commands.User.ApproveManager.ApproveManagerCommandHandler>();
        services.AddScoped<ICommandHandler<Commands.User.RejectManager.RejectManagerCommand, Domain.Entities.User>, Commands.User.RejectManager.RejectManagerCommandHandler>();
        services.AddScoped<ICommandHandler<Commands.Camping.CreateCamping.CreateCampingCommand, Domain.Entities.Camping>, Commands.Camping.CreateCamping.CreateCampingCommandHandler>();
        services.AddScoped<ICommandHandler<Commands.Camping.UpdateCamping.UpdateCampingCommand, Domain.Entities.Camping>, Commands.Camping.UpdateCamping.UpdateCampingCommandHandler>();
        services.AddScoped<ICommandHandler<Commands.Camping.DeleteCamping.DeleteCampingCommand>, Commands.Camping.DeleteCamping.DeleteCampingCommandHandler>();
        services.AddScoped<ICommandHandler<Commands.Favorite.AddFavorite.AddFavoriteCommand, Domain.Entities.Favorite>, Commands.Favorite.AddFavorite.AddFavoriteCommandHandler>();
        services.AddScoped<ICommandHandler<Commands.Favorite.RemoveFavorite.RemoveFavoriteCommand>, Commands.Favorite.RemoveFavorite.RemoveFavoriteCommandHandler>();
        services.AddScoped<ICommandHandler<Commands.Reservation.CreateReservation.CreateReservationCommand, Domain.Entities.Reservation>, Commands.Reservation.CreateReservation.CreateReservationCommandHandler>();
        services.AddScoped<ICommandHandler<Commands.Reservation.CancelReservation.CancelReservationCommand>, Commands.Reservation.CancelReservation.CancelReservationCommandHandler>();
    }


}
