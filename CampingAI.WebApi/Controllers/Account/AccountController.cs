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
    readonly Application.Services.PasswordHashingService.Interfaces.IPasswordHashingService _passwordHashingService;
    #endregion

    public AccountController(Domain.Repositories.IUsersReadRepository usersReadRepository,
                             Application.Services.PasswordHashingService.Interfaces.IPasswordHashingService passwordHashingService) {
        _usersReadRepository = usersReadRepository;
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

        return RedirectToAction("Profile");
    }

    [HttpPost]
    public IActionResult GoogleLogin(string? returnUrl = null) {
        var redirectUrl = Url.Action("GoogleCallback", "Account", new { returnUrl });
        var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }

    public async Task<IActionResult> GoogleCallback(string? returnUrl = null) {
        var authenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        if (!authenticateResult.Succeeded) {
            return RedirectToAction("Login");
        }

        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return RedirectToAction("Profile");
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