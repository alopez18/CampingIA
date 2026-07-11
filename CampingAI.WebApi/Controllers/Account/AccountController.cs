using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CampingAI.WebApi.Controllers.Account;
public class AccountController : Controller {
    public IActionResult Login() {
        return View();
    }

    [HttpPost]
    public IActionResult GoogleLogin() {
        var redirectUrl = Url.Action("GoogleCallback", "Account");
        var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }

    public async Task<IActionResult> GoogleCallback() {
        var authenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        if (!authenticateResult.Succeeded) {
            return RedirectToAction("Login");
        }

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