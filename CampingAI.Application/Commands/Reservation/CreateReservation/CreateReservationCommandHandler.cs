using FluentValidation;

namespace CampingAI.Application.Commands.Reservation.CreateReservation;
public class CreateReservationCommandHandler : Abstractions.Command.ICommandHandler<CreateReservationCommand, Domain.Entities.Reservation> {

    #region Dependencias
    readonly Domain.Repositories.IReservationsWriteRepository _reservationsWriteRepository;
    readonly IValidator<CreateReservationCommand> _validator;
    #endregion

    public CreateReservationCommandHandler(Domain.Repositories.IReservationsWriteRepository reservationsWriteRepository,
                                           IValidator<CreateReservationCommand> validator) {
        _reservationsWriteRepository = reservationsWriteRepository;
        _validator = validator;
    }

    public async Task<Domain.Entities.Reservation> HandleAsync(CreateReservationCommand command) {
        await _validator.ValidateAndThrowAsync(command);

        var reservation = Domain.Entities.Reservation.CreateNew(
            command.UserId,
            command.CampingId,
            command.CheckIn,
            command.CheckOut,
            command.TotalPrice,
            (int)Domain.Enums.ReservationStatus.Pending);

        await _reservationsWriteRepository.AddAsync(reservation);

        return reservation;
    }
}
