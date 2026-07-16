using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CampingAI.WebApi.Controllers.Admin;

[Authorize(Roles = "Admin")]
public class AdminController : Controller {

    #region Dependencias
    readonly Application.Abstractions.Query.IQueryHandler<Application.Queries.User.GetPendingManagers.GetPendingManagersQuery, IEnumerable<Domain.Entities.User>> _getPendingManagersQueryHandler;
    readonly Application.Abstractions.Command.ICommandHandler<Application.Commands.User.ApproveManager.ApproveManagerCommand, Domain.Entities.User> _approveManagerCommandHandler;
    readonly Application.Abstractions.Command.ICommandHandler<Application.Commands.User.RejectManager.RejectManagerCommand, Domain.Entities.User> _rejectManagerCommandHandler;
    #endregion

    public AdminController(Application.Abstractions.Query.IQueryHandler<Application.Queries.User.GetPendingManagers.GetPendingManagersQuery, IEnumerable<Domain.Entities.User>> getPendingManagersQueryHandler,
                           Application.Abstractions.Command.ICommandHandler<Application.Commands.User.ApproveManager.ApproveManagerCommand, Domain.Entities.User> approveManagerCommandHandler,
                           Application.Abstractions.Command.ICommandHandler<Application.Commands.User.RejectManager.RejectManagerCommand, Domain.Entities.User> rejectManagerCommandHandler) {
        _getPendingManagersQueryHandler = getPendingManagersQueryHandler;
        _approveManagerCommandHandler = approveManagerCommandHandler;
        _rejectManagerCommandHandler = rejectManagerCommandHandler;
    }

    [HttpGet]
    public async Task<IActionResult> PendingManagers() {
        var pending = await _getPendingManagersQueryHandler.HandleAsync(new Application.Queries.User.GetPendingManagers.GetPendingManagersQuery());
        return View(pending);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(Guid id) {
        await _approveManagerCommandHandler.HandleAsync(new Application.Commands.User.ApproveManager.ApproveManagerCommand(id));
        return RedirectToAction("PendingManagers");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(Guid id) {
        await _rejectManagerCommandHandler.HandleAsync(new Application.Commands.User.RejectManager.RejectManagerCommand(id));
        return RedirectToAction("PendingManagers");
    }
}
