using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CampingAI.WebApi.Controllers.Gestor;

[Authorize]
public class GestorController : Controller {

    #region Dependencias
    readonly Application.Abstractions.Command.ICommandHandler<Application.Commands.User.RequestManagerRole.RequestManagerRoleCommand, Domain.Entities.User> _requestManagerRoleCommandHandler;
    #endregion

    public GestorController(Application.Abstractions.Command.ICommandHandler<Application.Commands.User.RequestManagerRole.RequestManagerRoleCommand, Domain.Entities.User> requestManagerRoleCommandHandler) {
        _requestManagerRoleCommandHandler = requestManagerRoleCommandHandler;
    }

    [HttpGet]
    public IActionResult BecomeGestor(string? returnUrl = null) {
        if (User.IsInRole("Gestor") || User.IsInRole("Admin"))
            return RedirectToAction("Index", "Backoffice");

        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmBecomeGestor() {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdClaim, out var userId))
            return RedirectToAction("Login", "Account");

        try {
            await _requestManagerRoleCommandHandler.HandleAsync(
                new Application.Commands.User.RequestManagerRole.RequestManagerRoleCommand(userId));
        } catch (KeyNotFoundException) {
            return RedirectToAction("Login", "Account");
        }

        return RedirectToAction("RegisterPending", "Manager");
    }
}
