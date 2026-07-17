using FluentAssertions;
using FluentValidation;
using Moq;
using CampingAI.Application.Commands.Camping.UpdateCamping;
using CampingAI.Domain.Repositories;

namespace CampingAI.Application.Tests.Commands.Camping;
public class UpdateCampingCommandHandlerTests {

    private readonly Mock<ICampingsReadRepository> _readRepositoryMock;
    private readonly Mock<ICampingsWriteRepository> _writeRepositoryMock;
    private readonly Mock<ICampingCategoriesWriteRepository> _categoriesWriteRepositoryMock;
    private readonly Mock<ICampingFacilitiesWriteRepository> _facilitiesWriteRepositoryMock;
    private readonly Mock<IValidator<UpdateCampingCommand>> _validatorMock;
    private readonly UpdateCampingCommandHandler _handler;

    public UpdateCampingCommandHandlerTests() {
        _readRepositoryMock = new Mock<ICampingsReadRepository>();
        _writeRepositoryMock = new Mock<ICampingsWriteRepository>();
        _categoriesWriteRepositoryMock = new Mock<ICampingCategoriesWriteRepository>();
        _facilitiesWriteRepositoryMock = new Mock<ICampingFacilitiesWriteRepository>();
        _validatorMock = new Mock<IValidator<UpdateCampingCommand>>();

        _validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<UpdateCampingCommand>>(), default))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _handler = new UpdateCampingCommandHandler(
            _readRepositoryMock.Object,
            _writeRepositoryMock.Object,
            _categoriesWriteRepositoryMock.Object,
            _facilitiesWriteRepositoryMock.Object,
            _validatorMock.Object);
    }

    [Fact]
    public async Task HandleAsync_Should_UpdateCamping_WhenCampingExists() {
        // Arrange
        var campingId = Guid.NewGuid();
        var existingCamping = Domain.Entities.Camping.CreateNew(
            "Original", "Desc", 40m, -3m, 20m, Guid.NewGuid(), Guid.NewGuid());

        _readRepositoryMock
            .Setup(r => r.GetByIdAsync(campingId))
            .ReturnsAsync(existingCamping);

        var newCategoryId = Guid.NewGuid();
        var command = new UpdateCampingCommand(campingId, "Actualizado", "Nueva Desc", 41m, -2m, 35m, newCategoryId, null, null, null);

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        result.Should().NotBeNull();
        result.Name.ToString().Should().Be("Actualizado");
        result.PricePerNight.Value.Should().Be(35m);
        result.Latitude.Value.Should().Be(41m);
        _writeRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Domain.Entities.Camping>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_Should_ThrowKeyNotFoundException_WhenCampingDoesNotExist() {
        // Arrange
        var campingId = Guid.NewGuid();
        _readRepositoryMock
            .Setup(r => r.GetByIdAsync(campingId))
            .ReturnsAsync((Domain.Entities.Camping?)null);

        var command = new UpdateCampingCommand(campingId, "N", "D", 40m, -3m, 20m, Guid.NewGuid(), null, null, null);

        // Act
        Func<Task> act = () => _handler.HandleAsync(command);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"*{campingId}*");
    }
}
