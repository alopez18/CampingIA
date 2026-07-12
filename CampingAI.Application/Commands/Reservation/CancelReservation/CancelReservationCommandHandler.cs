using FluentValidation;

namespace CampingAI.Application.Commands.Reservation.CancelReservation;
public class CancelReservationCommandHandler : Abstractions.Command.ICommandHandler<CancelReservationCommand> {

    #region Dependencias
    readonly Infra.Abstractions.IUnitOfWork _unitOfWork;
    readonly Domain.Repositories.IReservationsReadRepository _reservationsReadRepository;
    readonly Domain.Repositories.IReservationsWriteRepository _reservationsWriteRepository;
    readonly IValidator<CancelReservationCommand> _validator;
    #endregion

    public CancelReservationCommandHandler(Infra.Abstractions.IUnitOfWork unitOfWork,
                                           Domain.Repositories.IReservationsReadRepository reservationsReadRepository,
                                           Domain.Repositories.IReservationsWriteRepository reservationsWriteRepository,
                                           IValidator<CancelReservationCommand> validator) {
        _unitOfWork = unitOfWork;
        _reservationsReadRepository = reservationsReadRepository;
        _reservationsWriteRepository = reservationsWriteRepository;
        _validator = validator;
    }

    public async Task HandleAsync(CancelReservationCommand command) {
        await _validator.ValidateAndThrowAsync(command);

        var reservation = await _reservationsReadRepository.GetByIdAsync(command.ReservationId)
            ?? throw new Domain.Exceptions.DomainException("Reserva no encontrada.");

        if (reservation.UserId != command.UserId)
            throw new Domain.Exceptions.DomainException("No tienes permiso para cancelar esta reserva.");

        reservation.Cancel();

        await _reservationsWriteRepository.UpdateAsync(reservation);
        await _unitOfWork.SaveChangesAsync();
    }
}
