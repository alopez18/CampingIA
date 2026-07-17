using FluentAssertions;
using FluentValidation;
using Moq;
using CampingAI.Application.Commands.Camping.CreateCamping;
using CampingAI.Domain.Repositories;

namespace CampingAI.Application.Tests.Commands.Camping;
public class CreateCampingCommandHandlerTests {

    private readonly Mock<ICampingsWriteRepository> _writeRepositoryMock;
    private readonly Mock<ICampingCategoriesWriteRepository> _categoriesWriteRepositoryMock;
    private readonly Mock<IValidator<CreateCampingCommand>> _validatorMock;
    private readonly CreateCampingCommandHandler _handler;

    public CreateCampingCommandHandlerTests() {
        _writeRepositoryMock = new Mock<ICampingsWriteRepository>();
        _categoriesWriteRepositoryMock = new Mock<ICampingCategoriesWriteRepository>();
        _validatorMock = new Mock<IValidator<CreateCampingCommand>>();

        _validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<CreateCampingCommand>>(), default))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _handler = new CreateCampingCommandHandler(
            _writeRepositoryMock.Object,
            _categoriesWriteRepositoryMock.Object,
            _validatorMock.Object);
    }

    [Fact]
    public async Task HandleAsync_Should_CreateCamping_WhenCommandIsValid() {
        // Arrange
        var command = new CreateCampingCommand(
            "Camping El Bosque",
            "Un camping en el bosque",
            40.4m,
            -3.7m,
            25m,
            Guid.NewGuid(),
            Guid.NewGuid(),
            null,
            null,
            null);

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBe(Guid.Empty);
        result.Name.ToString().Should().Be(command.Name);
        result.PricePerNight.Value.Should().Be(command.PricePerNight);
        _writeRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Domain.Entities.Camping>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_Should_SetFacilitiesAndCategories_WhenProvided() {
        // Arrange
        var f1 = Guid.NewGuid();
        var f2 = Guid.NewGuid();
        var cat1 = Guid.NewGuid();
        var command = new CreateCampingCommand(
            "Camping Sol",
            "Descripción",
            41.0m,
            2.0m,
            30m,
            Guid.NewGuid(),
            Guid.NewGuid(),
            null,
            [f1, f2],
            [cat1]);

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        result.FacilityIds.Should().BeEquivalentTo(new[] { f1, f2 });
        result.AdditionalCategoryIds.Should().Contain(cat1);
        _categoriesWriteRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Domain.Entities.CampingCategory>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_Should_ThrowValidationException_WhenValidatorFails() {
        // Arrange
        _validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<CreateCampingCommand>>(), default))
            .ThrowsAsync(new ValidationException("Name is required"));

        var command = new CreateCampingCommand("valid name", "Desc", 40m, -3m, 25m, Guid.NewGuid(), Guid.NewGuid(), null, null, null);

        // Act
        Func<Task> act = () => _handler.HandleAsync(command);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }
}
