using FluentAssertions;
using FluentValidation;
using Moq;
using CampingAI.Application.Commands.Camping.DeleteCamping;
using CampingAI.Domain.Repositories;
using CampingAI.Infra.Abstractions;

namespace CampingAI.Application.Tests.Commands.Camping;
public class DeleteCampingCommandHandlerTests {

    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ICampingsReadRepository> _readRepositoryMock;
    private readonly Mock<ICampingsWriteRepository> _writeRepositoryMock;
    private readonly Mock<IValidator<DeleteCampingCommand>> _validatorMock;
    private readonly DeleteCampingCommandHandler _handler;

    public DeleteCampingCommandHandlerTests() {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _readRepositoryMock = new Mock<ICampingsReadRepository>();
        _writeRepositoryMock = new Mock<ICampingsWriteRepository>();
        _validatorMock = new Mock<IValidator<DeleteCampingCommand>>();

        _validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<DeleteCampingCommand>>(), default))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _handler = new DeleteCampingCommandHandler(
            _readRepositoryMock.Object,
            _writeRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _validatorMock.Object);
    }

    [Fact]
    public async Task HandleAsync_Should_DeleteCamping_WhenCampingExists() {
        // Arrange
        var campingId = Guid.NewGuid();
        var existingCamping = Domain.Entities.Camping.CreateNew(
            "Camping", "Desc", 40m, -3m, 25m, Guid.NewGuid(), Guid.NewGuid());

        _readRepositoryMock
            .Setup(r => r.GetByIdAsync(campingId))
            .ReturnsAsync(existingCamping);

        var command = new DeleteCampingCommand(campingId);

        // Act
        await _handler.HandleAsync(command);

        // Assert
        _writeRepositoryMock.Verify(r => r.DeleteAsync(existingCamping.Id), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_Should_ThrowKeyNotFoundException_WhenCampingDoesNotExist() {
        // Arrange
        var campingId = Guid.NewGuid();
        _readRepositoryMock
            .Setup(r => r.GetByIdAsync(campingId))
            .ReturnsAsync((Domain.Entities.Camping?)null);

        var command = new DeleteCampingCommand(campingId);

        // Act
        Func<Task> act = () => _handler.HandleAsync(command);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"*{campingId}*");
    }
}
