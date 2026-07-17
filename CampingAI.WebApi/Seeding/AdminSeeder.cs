using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CampingAI.WebApi.Seeding;

public static class AdminSeeder {
    public static async Task SeedAsync(IServiceProvider serviceProvider) {
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;

        var config = services.GetRequiredService<IConfiguration>();
        var logger = services.GetRequiredService<ILogger<Program>>();
        var usersReadRepository = services.GetRequiredService<Domain.Repositories.IUsersReadRepository>();
        var usersWriteRepository = services.GetRequiredService<Domain.Repositories.IUsersWriteRepository>();
        var passwordHashingService = services.GetRequiredService<Application.Services.PasswordHashingService.Interfaces.IPasswordHashingService>();

        var email = config["AdminSeed:Email"];
        var password = config["AdminSeed:Password"];

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password)) {
            logger.LogWarning("AdminSeed:Email o AdminSeed:Password no configurados. Se omite el seeding del Admin.");
            return;
        }

        try {
            var exists = await usersReadRepository.ExistsAsync(email);
            if (exists) {
                logger.LogInformation("El usuario Admin '{Email}' ya existe. Se omite el seeding.", email);
                return;
            }

            var passwordHashed = passwordHashingService.HashPassword(password);
            var admin = Domain.Entities.User.CreateNew(email, passwordHashed!, "Administrador", Domain.Enums.UserRole.Admin);

            await usersWriteRepository.AddAsync(admin);

            logger.LogInformation("Usuario Admin '{Email}' creado correctamente.", email);
        } catch (Exception ex) {
            logger.LogError(ex, "Error al crear el usuario Admin '{Email}'.", email);
        }
    }
}
