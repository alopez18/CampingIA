using FluentAssertions;
using FluentValidation;
using Moq;
using CampingAI.Application.Commands.Reservation.CreateReservation;
using CampingAI.Domain.Repositories;
using CampingAI.Infra.Abstractions;

namespace CampingAI.Application.Tests.Commands.Reservation;
public class CreateReservationCommandHandlerTests {

    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IReservationsWriteRepository> _writeRepositoryMock;
    private readonly Mock<IValidator<CreateReservationCommand>> _validatorMock;
    private readonly CreateReservationCommandHandler _handler;

    public CreateReservationCommandHandlerTests() {
        _unitOfWorkMock    = new Mock<IUnitOfWork>();
        _writeRepositoryMock = new Mock<IReservationsWriteRepository>();
        _validatorMock     = new Mock<IValidator<CreateReservationCommand>>();

        _validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<CreateReservationCommand>>(), default))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _handler = new CreateReservationCommandHandler(
            _unitOfWorkMock.Object,
            _writeRepositoryMock.Object,
            _validatorMock.Object);
    }

    [Fact]
    public async Task HandleAsync_Should_CreateReservation_WhenCommandIsValid() {
        // Arrange
        var command = new CreateReservationCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.UtcNow.Date.AddDays(1),
            DateTime.UtcNow.Date.AddDays(4),
            120m);

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        result.Should().NotBeNull();
        result.UserId.Should().Be(command.UserId);
        result.CampingId.Should().Be(command.CampingId);
        result.StatusId.Should().Be((int)Domain.Enums.ReservationStatus.Pending);
        _writeRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Domain.Entities.Reservation>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(CancellationToken.None), Times.Once);
    }
}
