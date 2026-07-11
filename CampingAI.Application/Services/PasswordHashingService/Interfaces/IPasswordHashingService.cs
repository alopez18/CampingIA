namespace CampingAI.Application.Services.PasswordHashingService.Interfaces;
public interface IPasswordHashingService {
    /// <summary>
    /// Hashea una contraseña usando BCrypt
    /// </summary>
    /// <param name="password">Contraseña en texto plano</param>
    /// <returns>Hash de la contraseña</returns>
    string? HashPassword(string? password);

    /// <summary>
    /// Verifica si una contraseña coincide con el hash almacenado
    /// </summary>
    /// <param name="password">Contraseña en texto plano</param>
    /// <param name="hashedPassword">Hash almacenado en la base de datos</param>
    /// <returns>True si la contraseña es correcta</returns>
    bool VerifyPassword(string password, string hashedPassword);

    /// <summary>
    /// Verifica si un hash necesita ser rehecho (por cambio en el costo)
    /// </summary>
    /// <param name="hashedPassword">Hash a verificar</param>
    /// <returns>True si necesita rehashing</returns>
    bool NeedsRehashing(string hashedPassword);
}
