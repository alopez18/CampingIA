using FluentValidation;

namespace CampingAI.Application.Commands.Camping.UpdateCamping;
public class UpdateCampingCommandHandler : Abstractions.Command.ICommandHandler<UpdateCampingCommand, Domain.Entities.Camping> {

    #region Dependencias
    readonly Domain.Repositories.ICampingsReadRepository _campingsReadRepository;
    readonly Domain.Repositories.ICampingsWriteRepository _campingsWriteRepository;
    readonly Domain.Repositories.ICampingCategoriesWriteRepository _campingCategoriesWriteRepository;
    readonly Domain.Repositories.ICampingFacilitiesWriteRepository _campingFacilitiesWriteRepository;
    readonly Infra.Abstractions.IUnitOfWork _unitOfWork;
    readonly IValidator<UpdateCampingCommand> _validator;
    #endregion

    public UpdateCampingCommandHandler(Domain.Repositories.ICampingsReadRepository campingsReadRepository,
                                       Domain.Repositories.ICampingsWriteRepository campingsWriteRepository,
                                       Domain.Repositories.ICampingCategoriesWriteRepository campingCategoriesWriteRepository,
                                       Domain.Repositories.ICampingFacilitiesWriteRepository campingFacilitiesWriteRepository,
                                       Infra.Abstractions.IUnitOfWork unitOfWork,
                                       IValidator<UpdateCampingCommand> validator) {
        _campingsReadRepository = campingsReadRepository;
        _campingsWriteRepository = campingsWriteRepository;
        _campingCategoriesWriteRepository = campingCategoriesWriteRepository;
        _campingFacilitiesWriteRepository = campingFacilitiesWriteRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<Domain.Entities.Camping> HandleAsync(UpdateCampingCommand command) {
        await _validator.ValidateAndThrowAsync(command);

        var camping = await _campingsReadRepository.GetByIdAsync(command.CampingId)
            ?? throw new KeyNotFoundException($"No existe ningún camping con el id '{command.CampingId}'.");

        camping.UpdateDetails(command.Name, command.Description, command.PricePerNight, command.CategoryId,
                              command.ProvinciaId);
        camping.UpdateLocation(command.Latitude, command.Longitude);

        if (command.FacilityIds is not null)
            camping.SetFacilities(command.FacilityIds);

        if (command.AdditionalCategoryIds is not null)
            camping.SetAdditionalCategories(command.AdditionalCategoryIds);

        camping.Updated();

        await _unitOfWork.BeginTransactionAsync();
        try {
            await _campingsWriteRepository.UpdateAsync(camping);

            await _campingCategoriesWriteRepository.DeleteByCampingIdAsync(camping.Id);
            foreach (var categoryId in camping.AdditionalCategoryIds)
                await _campingCategoriesWriteRepository.AddAsync(
                    Domain.Entities.CampingCategory.CreateNew(camping.Id, categoryId));

            await _campingFacilitiesWriteRepository.DeleteByCampingIdAsync(camping.Id);
            foreach (var facilityId in camping.FacilityIds)
                await _campingFacilitiesWriteRepository.AddAsync(
                    Domain.Entities.CampingFacility.CreateNew(camping.Id, facilityId));

            await _unitOfWork.CommitAsync();
        }
        catch {
            await _unitOfWork.RollbackAsync();
            throw;
        }

        return camping;
    }
}
