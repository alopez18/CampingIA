using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace CampingAI.WebApi.Controllers.Backoffice;

[Authorize(Roles = "Gestor")]
public class BackofficeController : Controller {

    #region Dependencias
    readonly Application.Abstractions.Query.IQueryHandler<Application.Queries.Camping.GetCampingsByOwner.GetCampingsByOwnerQuery, IEnumerable<Domain.Entities.Camping>> _getCampingsByOwnerQueryHandler;
    readonly Application.Abstractions.Query.IQueryHandler<Application.Queries.Camping.GetCampingById.GetCampingByIdQuery, Domain.Entities.Camping> _getCampingByIdQueryHandler;
    readonly Application.Abstractions.Command.ICommandHandler<Application.Commands.Camping.CreateCamping.CreateCampingCommand, Domain.Entities.Camping> _createCampingCommandHandler;
    readonly Application.Abstractions.Command.ICommandHandler<Application.Commands.Camping.UpdateCamping.UpdateCampingCommand, Domain.Entities.Camping> _updateCampingCommandHandler;
    readonly Application.Abstractions.Command.ICommandHandler<Application.Commands.Camping.DeleteCamping.DeleteCampingCommand> _deleteCampingCommandHandler;
    readonly Application.Abstractions.Query.IQueryHandler<Application.Queries.Category.GetCategories.GetCategoriesQuery, Application.Queries.Category.GetCategories.GetCategoriesResult> _getCategoriesQueryHandler;
    readonly Application.Abstractions.Query.IQueryHandler<Application.Queries.Location.GetProvinces.GetProvincesQuery, Application.Queries.Location.GetProvinces.GetProvincesResult> _getProvincesQueryHandler;
    readonly Application.Abstractions.Query.IQueryHandler<Application.Queries.Facility.GetFacilities.GetFacilitiesQuery, Application.Queries.Facility.GetFacilities.GetFacilitiesResult> _getFacilitiesQueryHandler;
    #endregion

    public BackofficeController(Application.Abstractions.Query.IQueryHandler<Application.Queries.Camping.GetCampingsByOwner.GetCampingsByOwnerQuery, IEnumerable<Domain.Entities.Camping>> getCampingsByOwnerQueryHandler,
                                Application.Abstractions.Query.IQueryHandler<Application.Queries.Camping.GetCampingById.GetCampingByIdQuery, Domain.Entities.Camping> getCampingByIdQueryHandler,
                                Application.Abstractions.Command.ICommandHandler<Application.Commands.Camping.CreateCamping.CreateCampingCommand, Domain.Entities.Camping> createCampingCommandHandler,
                                Application.Abstractions.Command.ICommandHandler<Application.Commands.Camping.UpdateCamping.UpdateCampingCommand, Domain.Entities.Camping> updateCampingCommandHandler,
                                Application.Abstractions.Command.ICommandHandler<Application.Commands.Camping.DeleteCamping.DeleteCampingCommand> deleteCampingCommandHandler,
                                Application.Abstractions.Query.IQueryHandler<Application.Queries.Category.GetCategories.GetCategoriesQuery, Application.Queries.Category.GetCategories.GetCategoriesResult> getCategoriesQueryHandler,
                                Application.Abstractions.Query.IQueryHandler<Application.Queries.Location.GetProvinces.GetProvincesQuery, Application.Queries.Location.GetProvinces.GetProvincesResult> getProvincesQueryHandler,
                                Application.Abstractions.Query.IQueryHandler<Application.Queries.Facility.GetFacilities.GetFacilitiesQuery, Application.Queries.Facility.GetFacilities.GetFacilitiesResult> getFacilitiesQueryHandler) {
        _getCampingsByOwnerQueryHandler = getCampingsByOwnerQueryHandler;
        _getCampingByIdQueryHandler = getCampingByIdQueryHandler;
        _createCampingCommandHandler = createCampingCommandHandler;
        _updateCampingCommandHandler = updateCampingCommandHandler;
        _deleteCampingCommandHandler = deleteCampingCommandHandler;
        _getCategoriesQueryHandler = getCategoriesQueryHandler;
        _getProvincesQueryHandler = getProvincesQueryHandler;
        _getFacilitiesQueryHandler = getFacilitiesQueryHandler;
    }

    Guid GetCurrentUserId() {
        var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(id, out var guid) ? guid : Guid.Empty;
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

    [HttpGet]
    public async Task<IActionResult> Index() {
        var campings = await _getCampingsByOwnerQueryHandler.HandleAsync(new Application.Queries.Camping.GetCampingsByOwner.GetCampingsByOwnerQuery(GetCurrentUserId()));
        return View(campings);
    }

    [HttpGet]
    public async Task<IActionResult> Create() {
        await LoadDropdownsAsync();
        return View("Edit", new DTO.CampingFormRequest());
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id) {
        var camping = await _getCampingByIdQueryHandler.HandleAsync(new Application.Queries.Camping.GetCampingById.GetCampingByIdQuery(id));
        if (camping is null || camping.OwnerId != GetCurrentUserId())
            return Forbid();

        await LoadDropdownsAsync();
        var model = new DTO.CampingFormRequest {
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
        return View("Edit", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(DTO.CampingFormRequest request) {
        if (!ModelState.IsValid) {
            await LoadDropdownsAsync();
            return View("Edit", request);
        }

        var ownerId = GetCurrentUserId();

        try {
            if (request.Id is null || request.Id == Guid.Empty) {
                var createCommand = new Application.Commands.Camping.CreateCamping.CreateCampingCommand(
                    request.Name, request.Description, request.Latitude, request.Longitude,
                    request.PricePerNight, ownerId, request.CategoryId, request.ProvinciaId,
                    request.FacilityIds, request.AdditionalCategoryIds);
                await _createCampingCommandHandler.HandleAsync(createCommand);
            } else {
                var existing = await _getCampingByIdQueryHandler.HandleAsync(new Application.Queries.Camping.GetCampingById.GetCampingByIdQuery(request.Id.Value));
                if (existing is null || existing.OwnerId != ownerId)
                    return Forbid();

                var updateCommand = new Application.Commands.Camping.UpdateCamping.UpdateCampingCommand(
                    request.Id.Value, request.Name, request.Description, request.Latitude, request.Longitude,
                    request.PricePerNight, request.CategoryId, request.ProvinciaId,
                    request.FacilityIds, request.AdditionalCategoryIds);
                await _updateCampingCommandHandler.HandleAsync(updateCommand);
            }
        } catch (Domain.Exceptions.DomainException ex) {
            ModelState.AddModelError(string.Empty, ex.Message);
            await LoadDropdownsAsync();
            return View("Edit", request);
        }

        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id) {
        var existing = await _getCampingByIdQueryHandler.HandleAsync(new Application.Queries.Camping.GetCampingById.GetCampingByIdQuery(id));
        if (existing is null || existing.OwnerId != GetCurrentUserId())
            return Forbid();

        await _deleteCampingCommandHandler.HandleAsync(new Application.Commands.Camping.DeleteCamping.DeleteCampingCommand(id));
        return RedirectToAction("Index");
    }
}
