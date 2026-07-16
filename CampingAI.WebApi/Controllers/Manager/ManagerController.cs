using Microsoft.AspNetCore.Mvc;

namespace CampingAI.WebApi.Controllers.Manager;
public class ManagerController : Controller {

    #region Dependencias
    readonly Application.Abstractions.Command.ICommandHandler<Application.Commands.User.RegisterManager.RegisterManagerCommand, Domain.Entities.User> _registerManagerCommandHandler;
    #endregion

    public ManagerController(Application.Abstractions.Command.ICommandHandler<Application.Commands.User.RegisterManager.RegisterManagerCommand, Domain.Entities.User> registerManagerCommandHandler) {
        _registerManagerCommandHandler = registerManagerCommandHandler;
    }

    [HttpGet]
    public IActionResult Register() {
        return View(new DTO.RegisterManagerFormRequest());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(DTO.RegisterManagerFormRequest request) {
        if (!ModelState.IsValid)
            return View(request);

        try {
            var command = new Application.Commands.User.RegisterManager.RegisterManagerCommand(request.Email, request.Name, request.Password);
            await _registerManagerCommandHandler.HandleAsync(command);
        } catch (Domain.Exceptions.DomainException ex) {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(request);
        }

        return RedirectToAction("RegisterPending");
    }

    [HttpGet]
    public IActionResult RegisterPending() {
        return View();
    }
}
