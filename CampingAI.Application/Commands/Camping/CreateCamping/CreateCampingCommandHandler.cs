using FluentValidation;

namespace CampingAI.Application.Commands.Camping.CreateCamping;
public class CreateCampingCommandHandler : Abstractions.Command.ICommandHandler<CreateCampingCommand, Domain.Entities.Camping> {

    #region Dependencias
    readonly Infra.Abstractions.IUnitOfWork _unitOfWork;
    readonly Domain.Repositories.ICampingsWriteRepository _campingsWriteRepository;
    readonly IValidator<CreateCampingCommand> _validator;
    #endregion

    public CreateCampingCommandHandler(Domain.Repositories.ICampingsWriteRepository campingsWriteRepository,
                                       Infra.Abstractions.IUnitOfWork unitOfWork,
                                       IValidator<CreateCampingCommand> validator) {
        _campingsWriteRepository = campingsWriteRepository;
        _unitOfWork = unitOfWork;
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

        await _campingsWriteRepository.AddAsync(camping);
        await _unitOfWork.SaveChangesAsync();

        return camping;
    }
}
