using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CampingAI.WebApi.Controllers.Account;
public class AccountController : Controller {

    #region Dependencias
    readonly Domain.Repositories.IUsersReadRepository _usersReadRepository;
    readonly Domain.Repositories.IUsersWriteRepository _usersWriteRepository;
    readonly Application.Services.PasswordHashingService.Interfaces.IPasswordHashingService _passwordHashingService;
    #endregion

    public AccountController(Domain.Repositories.IUsersReadRepository usersReadRepository,
                             Domain.Repositories.IUsersWriteRepository usersWriteRepository,
                             Application.Services.PasswordHashingService.Interfaces.IPasswordHashingService passwordHashingService) {
        _usersReadRepository = usersReadRepository;
        _usersWriteRepository = usersWriteRepository;
        _passwordHashingService = passwordHashingService;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null) {
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(DTO.LoginFormRequest request) {
        var user = await _usersReadRepository.GetByEmailAsync(request.Email);
        if (user is null || !_passwordHashingService.VerifyPassword(request.Password, user.PasswordHashed.ToString())) {
            ModelState.AddModelError(string.Empty, "Email o contraseña incorrectos.");
            ViewBag.ReturnUrl = request.ReturnUrl;
            return View();
        }

        var principal = UserClaimsFactory.Build(user, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        if (!string.IsNullOrWhiteSpace(request.ReturnUrl) && Url.IsLocalUrl(request.ReturnUrl))
            return Redirect(request.ReturnUrl);

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public IActionResult GoogleLogin(string? returnUrl = null) {
        var redirectUrl = Url.Action("GoogleCallback", "Account", new { returnUrl });
        var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }

    public async Task<IActionResult> GoogleCallback(string? returnUrl = null) {
        var authenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        if (!authenticateResult.Succeeded)
            return RedirectToAction("Login");

        // Extraer email del claim de Google
        var email = authenticateResult.Principal?.FindFirst(ClaimTypes.Email)?.Value;
        var name = authenticateResult.Principal?.FindFirst(ClaimTypes.Name)?.Value;

        if (string.IsNullOrWhiteSpace(email)) {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            ModelState.AddModelError(string.Empty, "No se pudo obtener el email de la cuenta de Google.");
            return RedirectToAction("Login");
        }

        // Buscar o crear usuario en nuestra BD
        var user = await _usersReadRepository.GetByEmailAsync(email);
        if (user is null) {
            user = Domain.Entities.User.CreateNew(email, "GOOGLE_OAUTH", name, Domain.Enums.UserRole.Comun);
            await _usersWriteRepository.AddAsync(user);
        }

        // Cerrar sesión de Google y abrir sesión con nuestros claims (rol incluido)
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = UserClaimsFactory.Build(user, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return RedirectToAction("Index", "Home");
    }

    [Authorize]
    public IActionResult Profile() {
        var user = HttpContext.User;
        var idToken = user.FindFirst("access_token")?.Value;
        var userInfo = new {
            Name = user.FindFirst(ClaimTypes.Name)?.Value,
            Email = user.FindFirst(ClaimTypes.Email)?.Value,
            GoogleId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value,
            IdToken = idToken,
            Picture = user.FindFirst("picture")?.Value
        };

        ViewBag.UserInfo = userInfo;
        ViewBag.IdToken = idToken;
        return View();
    }

    public async Task<IActionResult> Logout() {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }
}