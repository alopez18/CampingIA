using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CampingAI.WebApi.Controllers.Admin;

[Authorize(Roles = "Admin")]
public class AdminController : Controller {

    #region Dependencias
    readonly Application.Abstractions.Query.IQueryHandler<Application.Queries.User.GetPendingManagers.GetPendingManagersQuery, IEnumerable<Domain.Entities.User>> _getPendingManagersQueryHandler;
    readonly Application.Abstractions.Command.ICommandHandler<Application.Commands.User.ApproveManager.ApproveManagerCommand, Domain.Entities.User> _approveManagerCommandHandler;
    readonly Application.Abstractions.Command.ICommandHandler<Application.Commands.User.RejectManager.RejectManagerCommand, Domain.Entities.User> _rejectManagerCommandHandler;
    readonly Application.Abstractions.Query.IQueryHandler<Application.Queries.Camping.GetCampings.GetCampingsQuery, Application.Queries.Camping.GetCampings.GetCampingsResult> _getCampingsQueryHandler;
    readonly Application.Abstractions.Query.IQueryHandler<Application.Queries.Camping.GetCampingById.GetCampingByIdQuery, Domain.Entities.Camping> _getCampingByIdQueryHandler;
    readonly Application.Abstractions.Command.ICommandHandler<Application.Commands.Camping.UpdateCamping.UpdateCampingCommand, Domain.Entities.Camping> _updateCampingCommandHandler;
    readonly Application.Abstractions.Command.ICommandHandler<Application.Commands.Camping.DeleteCamping.DeleteCampingCommand> _deleteCampingCommandHandler;
    readonly Application.Abstractions.Command.ICommandHandler<Application.Commands.Camping.CreateCamping.CreateCampingCommand, Domain.Entities.Camping> _createCampingCommandHandler;
    readonly Application.Abstractions.Query.IQueryHandler<Application.Queries.Category.GetCategories.GetCategoriesQuery, Application.Queries.Category.GetCategories.GetCategoriesResult> _getCategoriesQueryHandler;
    readonly Application.Abstractions.Query.IQueryHandler<Application.Queries.Location.GetProvinces.GetProvincesQuery, Application.Queries.Location.GetProvinces.GetProvincesResult> _getProvincesQueryHandler;
    readonly Application.Abstractions.Query.IQueryHandler<Application.Queries.Facility.GetFacilities.GetFacilitiesQuery, Application.Queries.Facility.GetFacilities.GetFacilitiesResult> _getFacilitiesQueryHandler;
    readonly Application.Abstractions.Query.IQueryHandler<Application.Queries.User.GetAllUsers.GetAllUsersQuery, Application.Queries.User.GetAllUsers.GetAllUsersResult> _getAllUsersQueryHandler;
    readonly Application.Abstractions.Query.IQueryHandler<Application.Queries.User.GetUserById.GetUserByIdQuery, Domain.Entities.User> _getUserByIdQueryHandler;
    readonly Application.Abstractions.Command.ICommandHandler<Application.Commands.User.AdminUpdateUser.AdminUpdateUserCommand, Domain.Entities.User> _adminUpdateUserCommandHandler;
    readonly Application.Abstractions.Command.ICommandHandler<Application.Commands.User.DeleteUser.DeleteUserCommand> _deleteUserCommandHandler;
    readonly Application.Abstractions.Command.ICommandHandler<Application.Commands.User.AdminCreateUser.AdminCreateUserCommand, Domain.Entities.User> _adminCreateUserCommandHandler;
    #endregion

    public AdminController(Application.Abstractions.Query.IQueryHandler<Application.Queries.User.GetPendingManagers.GetPendingManagersQuery, IEnumerable<Domain.Entities.User>> getPendingManagersQueryHandler,
                           Application.Abstractions.Command.ICommandHandler<Application.Commands.User.ApproveManager.ApproveManagerCommand, Domain.Entities.User> approveManagerCommandHandler,
                           Application.Abstractions.Command.ICommandHandler<Application.Commands.User.RejectManager.RejectManagerCommand, Domain.Entities.User> rejectManagerCommandHandler,
                           Application.Abstractions.Query.IQueryHandler<Application.Queries.Camping.GetCampings.GetCampingsQuery, Application.Queries.Camping.GetCampings.GetCampingsResult> getCampingsQueryHandler,
                           Application.Abstractions.Query.IQueryHandler<Application.Queries.Camping.GetCampingById.GetCampingByIdQuery, Domain.Entities.Camping> getCampingByIdQueryHandler,
                           Application.Abstractions.Command.ICommandHandler<Application.Commands.Camping.UpdateCamping.UpdateCampingCommand, Domain.Entities.Camping> updateCampingCommandHandler,
                           Application.Abstractions.Command.ICommandHandler<Application.Commands.Camping.DeleteCamping.DeleteCampingCommand> deleteCampingCommandHandler,
                           Application.Abstractions.Command.ICommandHandler<Application.Commands.Camping.CreateCamping.CreateCampingCommand, Domain.Entities.Camping> createCampingCommandHandler,
                           Application.Abstractions.Query.IQueryHandler<Application.Queries.Category.GetCategories.GetCategoriesQuery, Application.Queries.Category.GetCategories.GetCategoriesResult> getCategoriesQueryHandler,
                           Application.Abstractions.Query.IQueryHandler<Application.Queries.Location.GetProvinces.GetProvincesQuery, Application.Queries.Location.GetProvinces.GetProvincesResult> getProvincesQueryHandler,
                           Application.Abstractions.Query.IQueryHandler<Application.Queries.Facility.GetFacilities.GetFacilitiesQuery, Application.Queries.Facility.GetFacilities.GetFacilitiesResult> getFacilitiesQueryHandler,
                           Application.Abstractions.Query.IQueryHandler<Application.Queries.User.GetAllUsers.GetAllUsersQuery, Application.Queries.User.GetAllUsers.GetAllUsersResult> getAllUsersQueryHandler,
                           Application.Abstractions.Query.IQueryHandler<Application.Queries.User.GetUserById.GetUserByIdQuery, Domain.Entities.User> getUserByIdQueryHandler,
                           Application.Abstractions.Command.ICommandHandler<Application.Commands.User.AdminUpdateUser.AdminUpdateUserCommand, Domain.Entities.User> adminUpdateUserCommandHandler,
                           Application.Abstractions.Command.ICommandHandler<Application.Commands.User.DeleteUser.DeleteUserCommand> deleteUserCommandHandler,
                           Application.Abstractions.Command.ICommandHandler<Application.Commands.User.AdminCreateUser.AdminCreateUserCommand, Domain.Entities.User> adminCreateUserCommandHandler) {
        _getPendingManagersQueryHandler = getPendingManagersQueryHandler;
        _approveManagerCommandHandler = approveManagerCommandHandler;
        _rejectManagerCommandHandler = rejectManagerCommandHandler;
        _getCampingsQueryHandler = getCampingsQueryHandler;
        _getCampingByIdQueryHandler = getCampingByIdQueryHandler;
        _updateCampingCommandHandler = updateCampingCommandHandler;
        _deleteCampingCommandHandler = deleteCampingCommandHandler;
        _createCampingCommandHandler = createCampingCommandHandler;
        _getCategoriesQueryHandler = getCategoriesQueryHandler;
        _getProvincesQueryHandler = getProvincesQueryHandler;
        _getFacilitiesQueryHandler = getFacilitiesQueryHandler;
        _getAllUsersQueryHandler = getAllUsersQueryHandler;
        _getUserByIdQueryHandler = getUserByIdQueryHandler;
        _adminUpdateUserCommandHandler = adminUpdateUserCommandHandler;
        _deleteUserCommandHandler = deleteUserCommandHandler;
        _adminCreateUserCommandHandler = adminCreateUserCommandHandler;
    }

    async Task LoadDropdownsAsync() {
        var categories = await _getCategoriesQueryHandler.HandleAsync(new Application.Queries.Category.GetCategories.GetCategoriesQuery());
        var provinces = await _getProvincesQueryHandler.HandleAsync(new Application.Queries.Location.GetProvinces.GetProvincesQuery(null));
        var facilities = await _getFacilitiesQueryHandler.HandleAsync(new Application.Queries.Facility.GetFacilities.GetFacilitiesQuery());

        ViewBag.Categories = categories.Items
            .Select(c => new SelectListItem(c.Name.ToString(), c.Id.ToString()))
            .ToList();

        ViewBag.Provinces = provinces.Items
            .OrderBy(p => p.Name.ToString())
            .Select(p => new SelectListItem(p.Name.ToString(), p.Id.ToString()))
            .ToList();

        ViewBag.Facilities = facilities.Items
            .OrderBy(f => f.Name.ToString())
            .Select(f => new SelectListItem(f.Name.ToString(), f.Id.ToString()))
            .ToList();
    }

    // --- Gestores ---

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

    // --- Campings ---

    Guid GetCurrentUserId() {
        var id = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(id, out var guid) ? guid : Guid.Empty;
    }

    [HttpGet]
    public async Task<IActionResult> Campings(int page = 1, string? search = null) {
        const int pageSize = 20;
        var result = await _getCampingsQueryHandler.HandleAsync(new Application.Queries.Camping.GetCampings.GetCampingsQuery(page, pageSize, search));
        ViewBag.Page = page;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalCount = result.TotalCount;
        ViewBag.TotalPages = (int)Math.Ceiling((double)result.TotalCount / pageSize);
        ViewBag.Search = search ?? string.Empty;
        return View(result.Items);
    }

    [HttpGet]
    public async Task<IActionResult> CreateCamping() {
        await LoadDropdownsAsync();
        return View("EditCamping", new Backoffice.DTO.CampingFormRequest());
    }

    [HttpGet]
    public async Task<IActionResult> EditCamping(Guid id) {
        var camping = await _getCampingByIdQueryHandler.HandleAsync(new Application.Queries.Camping.GetCampingById.GetCampingByIdQuery(id));
        if (camping is null)
            return NotFound();

        await LoadDropdownsAsync();
        var model = new Backoffice.DTO.CampingFormRequest {
            Id = camping.Id,
            Name = camping.Name.ToString(),
            Description = camping.Description.ToString(),
            Latitude = decimal.Parse(camping.Latitude.ToString()),
            Longitude = decimal.Parse(camping.Longitude.ToString()),
            PricePerNight = decimal.Parse(camping.PricePerNight.ToString()),
            CategoryId = camping.CategoryId,
            ProvinciaId = camping.ProvinciaId,
            AdditionalCategoryIds = camping.AdditionalCategoryIds.ToList(),
            FacilityIds = camping.FacilityIds.ToList()
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveCamping(Backoffice.DTO.CampingFormRequest request) {
        if (!ModelState.IsValid) {
            await LoadDropdownsAsync();
            return View("EditCamping", request);
        }

        try {
            if (request.Id is null || request.Id == Guid.Empty) {
                var createCommand = new Application.Commands.Camping.CreateCamping.CreateCampingCommand(
                    request.Name, request.Description, request.Latitude, request.Longitude,
                    request.PricePerNight, GetCurrentUserId(), request.CategoryId, request.ProvinciaId,
                    request.FacilityIds, request.AdditionalCategoryIds);
                await _createCampingCommandHandler.HandleAsync(createCommand);
            } else {
                var updateCommand = new Application.Commands.Camping.UpdateCamping.UpdateCampingCommand(
                    request.Id.Value, request.Name, request.Description, request.Latitude, request.Longitude,
                    request.PricePerNight, request.CategoryId, request.ProvinciaId,
                    request.FacilityIds, request.AdditionalCategoryIds);
                await _updateCampingCommandHandler.HandleAsync(updateCommand);
            }
        } catch (Domain.Exceptions.DomainException ex) {
            ModelState.AddModelError(string.Empty, ex.Message);
            await LoadDropdownsAsync();
            return View("EditCamping", request);
        }

        return RedirectToAction("Campings");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteCamping(Guid id) {
        var existing = await _getCampingByIdQueryHandler.HandleAsync(new Application.Queries.Camping.GetCampingById.GetCampingByIdQuery(id));
        if (existing is null)
            return NotFound();

        await _deleteCampingCommandHandler.HandleAsync(new Application.Commands.Camping.DeleteCamping.DeleteCampingCommand(id));
        return RedirectToAction("Campings");
    }

    // --- Usuarios ---

    [HttpGet]
    public async Task<IActionResult> Users(int page = 1, string? search = null) {
        const int pageSize = 20;
        var result = await _getAllUsersQueryHandler.HandleAsync(
            new Application.Queries.User.GetAllUsers.GetAllUsersQuery(page, pageSize, search));

        ViewBag.Page = page;
        ViewBag.TotalPages = (int)Math.Ceiling(result.TotalCount / (double)pageSize);
        ViewBag.TotalCount = result.TotalCount;
        ViewBag.Search = search;
        ViewBag.Roles = Enum.GetValues<Domain.Enums.UserRole>()
            .Select(r => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem(r.ToString(), ((int)r).ToString()))
            .ToList();

        return View(result.Items);
    }

    [HttpGet]
    public async Task<IActionResult> EditUser(Guid id) {
        var user = await _getUserByIdQueryHandler.HandleAsync(
            new Application.Queries.User.GetUserById.GetUserByIdQuery(id));

        ViewBag.Roles = Enum.GetValues<Domain.Enums.UserRole>()
            .Select(r => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem(r.ToString(), ((int)r).ToString()))
            .ToList();

        var model = new DTO.UserFormRequest {
            Id = user.Id,
            Name = user.Name ?? string.Empty,
            Email = user.Email.ToString(),
            RoleId = user.RoleId
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveUser(DTO.UserFormRequest request) {
        if (!ModelState.IsValid) {
            ViewBag.Roles = Enum.GetValues<Domain.Enums.UserRole>()
                .Select(r => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem(r.ToString(), ((int)r).ToString()))
                .ToList();
            return View("EditUser", request);
        }

        try {
            var role = (Domain.Enums.UserRole)request.RoleId;
            var command = new Application.Commands.User.AdminUpdateUser.AdminUpdateUserCommand(
                request.Id!.Value, request.Name, request.Email, role, request.NewPassword);
            await _adminUpdateUserCommandHandler.HandleAsync(command);
        } catch (Domain.Exceptions.DomainException ex) {
            ModelState.AddModelError(string.Empty, ex.Message);
            ViewBag.Roles = Enum.GetValues<Domain.Enums.UserRole>()
                .Select(r => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem(r.ToString(), ((int)r).ToString()))
                .ToList();
            return View("EditUser", request);
        }

        return RedirectToAction("Users");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteUser(Guid id) {
        await _deleteUserCommandHandler.HandleAsync(new Application.Commands.User.DeleteUser.DeleteUserCommand(id));
        return RedirectToAction("Users");
    }

    [HttpGet]
    public IActionResult CreateUser() {
        ViewBag.Roles = Enum.GetValues<Domain.Enums.UserRole>()
            .Select(r => new SelectListItem(r.ToString(), ((int)r).ToString()))
            .ToList();
        return View(new DTO.CreateUserFormRequest());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateUser(DTO.CreateUserFormRequest request) {
        if (!ModelState.IsValid) {
            ViewBag.Roles = Enum.GetValues<Domain.Enums.UserRole>()
                .Select(r => new SelectListItem(r.ToString(), ((int)r).ToString()))
                .ToList();
            return View(request);
        }

        try {
            var role = (Domain.Enums.UserRole)request.RoleId;
            var command = new Application.Commands.User.AdminCreateUser.AdminCreateUserCommand(
                request.Email, request.Password, request.Name, role);
            await _adminCreateUserCommandHandler.HandleAsync(command);
        } catch (Domain.Exceptions.DomainException ex) {
            ModelState.AddModelError(string.Empty, ex.Message);
            ViewBag.Roles = Enum.GetValues<Domain.Enums.UserRole>()
                .Select(r => new SelectListItem(r.ToString(), ((int)r).ToString()))
                .ToList();
            return View(request);
        }

        return RedirectToAction("Users");
    }
}
