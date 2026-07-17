using FluentValidation;

namespace CampingAI.Application.Commands.Camping.CreateCamping;
public class CreateCampingCommandHandler : Abstractions.Command.ICommandHandler<CreateCampingCommand, Domain.Entities.Camping> {

    #region Dependencias
    readonly Domain.Repositories.ICampingsWriteRepository _campingsWriteRepository;
    readonly Domain.Repositories.ICampingCategoriesWriteRepository _campingCategoriesWriteRepository;
    readonly IValidator<CreateCampingCommand> _validator;
    #endregion

    public CreateCampingCommandHandler(Domain.Repositories.ICampingsWriteRepository campingsWriteRepository,
                                       Domain.Repositories.ICampingCategoriesWriteRepository campingCategoriesWriteRepository,
                                       IValidator<CreateCampingCommand> validator) {
        _campingsWriteRepository = campingsWriteRepository;
        _campingCategoriesWriteRepository = campingCategoriesWriteRepository;
        _validator = validator;
    }

    public async Task<Domain.Entities.Camping> HandleAsync(CreateCampingCommand command) {
        await _validator.ValidateAndThrowAsync(command);

        var camping = Domain.Entities.Camping.CreateNew(command.Name,
                                                        command.Description,
                                                        command.Latitude,
                                                        command.Longitude,
                                                        command.PricePerNight,
                                                        command.OwnerId,
                                                        command.CategoryId,
                                                        command.ProvinciaId);

        if (command.FacilityIds is not null)
            camping.SetFacilities(command.FacilityIds);

        if (command.AdditionalCategoryIds is not null)
            camping.SetAdditionalCategories(command.AdditionalCategoryIds);

        await _campingsWriteRepository.AddAsync(camping);

        foreach (var categoryId in camping.AdditionalCategoryIds)
            await _campingCategoriesWriteRepository.AddAsync(
                    Domain.Entities.CampingCategory.CreateNew(camping.Id, categoryId));

            return camping;
    }
}
