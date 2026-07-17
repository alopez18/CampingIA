using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CampingAI.Infra.Configuration;

public static class DI_Manager
{
    public static void Configure(IServiceCollection services, IConfiguration config)
    {
        ContextConfiguration.ConfigureInfra(services, config);
        services.AddScoped<Abstractions.IUnitOfWork, UnitOfWork>();
        RegisterExtractors(services);
        RegisterMappers(services);
        RegisterRepositories(services);
    }

    private static void RegisterMappers(IServiceCollection services)
    {
        services.AddSingleton<Users.Mappers.UsersMapper>();
        services.AddSingleton<Campings.Mappers.CampingsMapper>();
        services.AddSingleton<Reservations.Mappers.ReservationsMapper>();
        services.AddSingleton<Facilities.Mappers.FacilitiesMapper>();
        services.AddSingleton<Favorites.Mappers.FavoritesMapper>();
        services.AddSingleton<Countries.Mappers.CountriesMapper>();
        services.AddSingleton<Provinces.Mappers.ProvincesMapper>();
        services.AddSingleton<Categories.Mappers.CategoriesMapper>();
    }

    private static void RegisterExtractors(IServiceCollection services)
    {
        services.AddSingleton<Abstractions.ModelExtractor<Models.CAMPING_AI_DB.T_EMPLOYEES>>();
        services.AddSingleton<Abstractions.ModelExtractor<Models.CampingAI_DB.T_USERS>>();
        services.AddSingleton<Abstractions.ModelExtractor<Models.CampingAI_DB.T_CAMPINGS>>();
        services.AddSingleton<Abstractions.ModelExtractor<Models.CampingAI_DB.T_RESERVATIONS>>();
        services.AddSingleton<Abstractions.ModelExtractor<Models.CampingAI_DB.T_FACILITIES>>();
        services.AddSingleton<Abstractions.ModelExtractor<Models.CampingAI_DB.T_CAMPING_FACILITIES>>();
        services.AddSingleton<Abstractions.ModelExtractor<Models.CampingAI_DB.T_FAVORITES>>();
        services.AddSingleton<Abstractions.ModelExtractor<Models.CampingAI_DB.T_COUNTRIES>>();
        services.AddSingleton<Abstractions.ModelExtractor<Models.CampingAI_DB.T_PROVINCES>>();
        services.AddSingleton<Abstractions.ModelExtractor<Models.CampingAI_DB.T_CATEGORIES>>();
        services.AddSingleton<Abstractions.ModelExtractor<Models.CampingAI_DB.T_CAMPING_CATEGORIES>>();
    }

    private static void RegisterRepositories(IServiceCollection services)
    {
        services.AddScoped<Domain.Repositories.IUsersReadRepository, Users.UsersReadRepository>();
        services.AddScoped<Domain.Repositories.IUsersWriteRepository, Users.UsersWriteRepository>();

        services.AddScoped<Domain.Repositories.ICampingsReadRepository, Campings.CampingsReadRepository>();
        services.AddScoped<Domain.Repositories.ICampingsWriteRepository, Campings.CampingsWriteRepository>();

        services.AddScoped<Domain.Repositories.IReservationsReadRepository, Reservations.ReservationsReadRepository>();
        services.AddScoped<Domain.Repositories.IReservationsWriteRepository, Reservations.ReservationsWriteRepository>();

        services.AddScoped<Domain.Repositories.IFacilitiesReadRepository, Facilities.FacilitiesReadRepository>();
        services.AddScoped<Domain.Repositories.IFacilitiesWriteRepository, Facilities.FacilitiesWriteRepository>();
        services.AddScoped<Domain.Repositories.ICampingFacilitiesWriteRepository, Facilities.CampingFacilitiesWriteRepository>();

        services.AddScoped<Domain.Repositories.IFavoritesReadRepository, Favorites.FavoritesReadRepository>();
        services.AddScoped<Domain.Repositories.IFavoritesWriteRepository, Favorites.FavoritesWriteRepository>();

        services.AddScoped<Domain.Repositories.ICountriesReadRepository, Countries.CountriesReadRepository>();
        services.AddScoped<Domain.Repositories.IProvincesReadRepository, Provinces.ProvincesReadRepository>();

        services.AddScoped<Domain.Repositories.ICategoriesReadRepository, Categories.CategoriesReadRepository>();
        services.AddScoped<Domain.Repositories.ICategoriesWriteRepository, Categories.CategoriesWriteRepository>();
        services.AddScoped<Domain.Repositories.ICampingCategoriesWriteRepository, Categories.CampingCategoriesWriteRepository>();
    }

}
