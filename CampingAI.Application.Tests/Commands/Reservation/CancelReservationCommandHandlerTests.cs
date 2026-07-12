using FluentAssertions;
using FluentValidation;
using Moq;
using CampingAI.Application.Commands.Reservation.CancelReservation;
using CampingAI.Domain.Repositories;
using CampingAI.Domain.Exceptions;
using CampingAI.Infra.Abstractions;

namespace CampingAI.Application.Tests.Commands.Reservation;
public class CancelReservationCommandHandlerTests {

    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IReservationsReadRepository> _readRepositoryMock;
    private readonly Mock<IReservationsWriteRepository> _writeRepositoryMock;
    private readonly Mock<IValidator<CancelReservationCommand>> _validatorMock;
    private readonly CancelReservationCommandHandler _handler;

    public CancelReservationCommandHandlerTests() {
        _unitOfWorkMock      = new Mock<IUnitOfWork>();
        _readRepositoryMock  = new Mock<IReservationsReadRepository>();
        _writeRepositoryMock = new Mock<IReservationsWriteRepository>();
        _validatorMock       = new Mock<IValidator<CancelReservationCommand>>();

        _validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<CancelReservationCommand>>(), default))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _handler = new CancelReservationCommandHandler(
            _unitOfWorkMock.Object,
            _readRepositoryMock.Object,
            _writeRepositoryMock.Object,
            _validatorMock.Object);
    }

    private static Domain.Entities.Reservation BuildReservation(Guid userId, int statusId = (int)Domain.Enums.ReservationStatus.Pending) {
        return new Domain.Entities.Reservation(
            Guid.NewGuid(), userId, Guid.NewGuid(),
            DateTime.Today.AddDays(2), DateTime.Today.AddDays(5),
            100m, statusId, DateTime.Now, DateTime.Now, null);
    }

    [Fact]
    public async Task HandleAsync_Should_CancelReservation_WhenOwnerRequests() {
        // Arrange
        var userId      = Guid.NewGuid();
        var reservation = BuildReservation(userId);
        var command     = new CancelReservationCommand(reservation.Id, userId);

        _readRepositoryMock.Setup(r => r.GetByIdAsync(reservation.Id)).ReturnsAsync(reservation);

        // Act
        await _handler.HandleAsync(command);

        // Assert
        reservation.StatusId.Should().Be((int)Domain.Enums.ReservationStatus.Cancelled);
        _writeRepositoryMock.Verify(r => r.UpdateAsync(reservation), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_Should_ThrowDomainException_WhenReservationNotFound() {
        // Arrange
        var command = new CancelReservationCommand(Guid.NewGuid(), Guid.NewGuid());
        _readRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Domain.Entities.Reservation?)null);

        // Act
        var act = async () => await _handler.HandleAsync(command);

        // Assert
        await act.Should().ThrowAsync<DomainException>();
        _writeRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Domain.Entities.Reservation>()), Times.Never);
    }

    [Fact]
    public async Task HandleAsync_Should_ThrowDomainException_WhenUserIsNotOwner() {
        // Arrange
        var reservation = BuildReservation(Guid.NewGuid());
        var command     = new CancelReservationCommand(reservation.Id, Guid.NewGuid()); // otro usuario

        _readRepositoryMock.Setup(r => r.GetByIdAsync(reservation.Id)).ReturnsAsync(reservation);

        // Act
        var act = async () => await _handler.HandleAsync(command);

        // Assert
        await act.Should().ThrowAsync<DomainException>();
        _writeRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Domain.Entities.Reservation>()), Times.Never);
    }
}
