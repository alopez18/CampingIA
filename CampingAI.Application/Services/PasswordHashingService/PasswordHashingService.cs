using Microsoft.Extensions.Configuration;
using CampingAI.Application.Services.PasswordHashingService.Interfaces;

namespace CampingAI.Application.Services.PasswordHashingService;
public class PasswordHashingService : IPasswordHashingService {
    private readonly int _workFactor;

    public PasswordHashingService(IConfiguration configuration) {

        IConfigurationSection section = configuration.GetSection("Security:BCryptWorkFactor");

        if (section == null
            || !int.TryParse(section.Value, out _workFactor)
            || _workFactor < 4
            || _workFactor > 31) {
            _workFactor = 12; // Valor por defecto si no está configurado correctamente
        }
    }

    public string? HashPassword(string? password) {
        if (string.IsNullOrWhiteSpace(password))
            return null;

        return BCrypt.Net.BCrypt.HashPassword(password, _workFactor);
    }

    public bool VerifyPassword(string password, string hashedPassword) {
        if (string.IsNullOrWhiteSpace(password))
            return false;

        if (string.IsNullOrWhiteSpace(hashedPassword))
            return false;

        try {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        } catch (Exception) {
            // En caso de hash malformado o error, devolver false
            return false;
        }
    }

    public bool NeedsRehashing(string hashedPassword) {
        if (string.IsNullOrWhiteSpace(hashedPassword))
            return true;

        try {
            return BCrypt.Net.BCrypt.PasswordNeedsRehash(hashedPassword, _workFactor);
        } catch (Exception) {
            // Si no se puede determinar, asumir que necesita rehashing
            return true;
        }
    }
}

