namespace CampingAI.WebApi.Config;
public static class DI_Manager {

    public static void Configure(IServiceCollection services, IConfiguration config) {
        RegisterMappers(services);
        RegisterServices(services, config);

        Application.Configuration.DI_Manager.Configure(services, config);
    }

    private static void RegisterServices(IServiceCollection services, IConfiguration config) {
        services.Configure<Settings.JwtSettings>(config.GetSection(Settings.JwtSettings.SECTION));
        services.AddScoped<Application.Services.JwtTokenService.Interfaces.IJwtTokenService, Services.JwtTokenService>();
    }

    private static void RegisterMappers(IServiceCollection services) {
        services.AddScoped<Controllers.api.Auth.Mappers.AuthResponseMapper>();
        services.AddScoped<Controllers.api.Users.Mappers.UserResponseMapper>();
        services.AddScoped<Controllers.api.Campings.Mappers.CampingResponseMapper>();
    }


}