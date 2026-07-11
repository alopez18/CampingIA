namespace CampingAI.WebApi.Settings;
public class JwtSettings {
    public const string SECTION = "Jwt";

    public string Issuer { get; init; } = string.Empty;
    public string Audience { get; init; } = string.Empty;
    public string Key { get; init; } = string.Empty;
}
