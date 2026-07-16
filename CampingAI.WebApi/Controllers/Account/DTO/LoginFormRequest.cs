namespace CampingAI.WebApi.Controllers.Account.DTO;
public class LoginFormRequest {
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? ReturnUrl { get; set; }
}
