using FluentValidation;

namespace CampingAI.Application.Commands.Camping.UpdateCamping;
public class UpdateCampingCommandHandler : Abstractions.Command.ICommandHandler<UpdateCampingCommand, Domain.Entities.Camping> {

    #region Dependencias
    readonly Infra.Abstractions.IUnitOfWork _unitOfWork;
    readonly Domain.Repositories.ICampingsReadRepository _campingsReadRepository;
    readonly Domain.Repositories.ICampingsWriteRepository _campingsWriteRepository;
    readonly IValidator<UpdateCampingCommand> _validator;
    #endregion

    public UpdateCampingCommandHandler(Domain.Repositories.ICampingsReadRepository campingsReadRepository,
                                       Domain.Repositories.ICampingsWriteRepository campingsWriteRepository,
                                       Infra.Abstractions.IUnitOfWork unitOfWork,
                                       IValidator<UpdateCampingCommand> validator) {
        _campingsReadRepository = campingsReadRepository;
        _campingsWriteRepository = campingsWriteRepository;
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<Domain.Entities.Camping> HandleAsync(UpdateCampingCommand command) {
        await _validator.ValidateAndThrowAsync(command);

        var camping = await _campingsReadRepository.GetByIdAsync(command.CampingId)
            ?? throw new KeyNotFoundException($"No existe ningún camping con el id '{command.CampingId}'.");

        camping.UpdateDetails(command.Name, command.Description, command.PricePerNight, command.CategoryId);
        camping.UpdateLocation(command.Latitude, command.Longitude);

        if (command.FacilityIds is not null)
            camping.SetFacilities(command.FacilityIds);

        camping.Updated();

        await _campingsWriteRepository.UpdateAsync(camping);
        await _unitOfWork.SaveChangesAsync();

        return camping;
    }
}
