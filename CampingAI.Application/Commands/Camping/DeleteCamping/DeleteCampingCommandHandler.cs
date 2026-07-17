using FluentValidation;

namespace CampingAI.Application.Commands.Camping.DeleteCamping;
public class DeleteCampingCommandHandler : Abstractions.Command.ICommandHandler<DeleteCampingCommand> {

    #region Dependencias
    readonly Domain.Repositories.ICampingsReadRepository _campingsReadRepository;
    readonly Domain.Repositories.ICampingsWriteRepository _campingsWriteRepository;
    readonly IValidator<DeleteCampingCommand> _validator;
    #endregion

    public DeleteCampingCommandHandler(Domain.Repositories.ICampingsReadRepository campingsReadRepository,
                                       Domain.Repositories.ICampingsWriteRepository campingsWriteRepository,
                                       IValidator<DeleteCampingCommand> validator) {
        _campingsReadRepository = campingsReadRepository;
        _campingsWriteRepository = campingsWriteRepository;
        _validator = validator;
    }

    public async Task HandleAsync(DeleteCampingCommand command) {
        await _validator.ValidateAndThrowAsync(command);

        var camping = await _campingsReadRepository.GetByIdAsync(command.CampingId)
            ?? throw new KeyNotFoundException($"No existe ningún camping con el id '{command.CampingId}'.");

        await _campingsWriteRepository.DeleteAsync(camping.Id);
    }
}
