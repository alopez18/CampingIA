using FluentAssertions;
using CampingAI.Domain.Entities;
using CampingAI.Domain.Exceptions;

namespace CampingAI.Domain.Tests.Entities;
public class CampingTests {

    private static Camping CreateSample(
        string name = "Camping El Bosque",
        string description = "Un camping en el bosque",
        decimal latitude = 40.4m,
        decimal longitude = -3.7m,
        decimal pricePerNight = 25m,
        Guid? ownerId = null,
        Guid? categoryId = null) {
        return Camping.CreateNew(
            name,
            description,
            latitude,
            longitude,
            pricePerNight,
            ownerId ?? Guid.NewGuid(),
            categoryId ?? Guid.NewGuid());
    }

    [Fact]
    public void CreateNew_Should_InitializeCampingWithCorrectValues() {
        // Arrange
        var ownerId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();

        // Act
        var camping = Camping.CreateNew("Camping Sol", "Descripción", 41.0m, 2.0m, 30m, ownerId, categoryId);

        // Assert
        camping.Should().NotBeNull();
        camping.Id.Should().NotBe(Guid.Empty);
        camping.Name.ToString().Should().Be("Camping Sol");
        camping.Description.ToString().Should().Be("Descripción");
        camping.Latitude.Value.Should().Be(41.0m);
        camping.Longitude.Value.Should().Be(2.0m);
        camping.PricePerNight.Value.Should().Be(30m);
        camping.OwnerId.Should().Be(ownerId);
        camping.CategoryId.Should().Be(categoryId);
        camping.DeletedOn.Should().BeNull();
        camping.FacilityIds.Should().BeEmpty();
        camping.AdditionalCategoryIds.Should().BeEmpty();
    }

    [Fact]
    public void CreateNew_Should_SetProvinciaId_WhenProvided() {
        // Arrange
        var provinciaId = Guid.NewGuid();

        // Act
        var camping = Camping.CreateNew("Camping", "Desc", 40m, -3m, 20m, Guid.NewGuid(), Guid.NewGuid(), provinciaId);

        // Assert
        camping.ProvinciaId.Should().Be(provinciaId);
    }

    [Fact]
    public void UpdateDetails_Should_UpdateNameDescriptionPriceAndCategory() {
        // Arrange
        var camping = CreateSample();
        var newCategoryId = Guid.NewGuid();

        // Act
        camping.UpdateDetails("Nuevo Nombre", "Nueva Descripción", 50m, newCategoryId);

        // Assert
        camping.Name.ToString().Should().Be("Nuevo Nombre");
        camping.Description.ToString().Should().Be("Nueva Descripción");
        camping.PricePerNight.Value.Should().Be(50m);
        camping.CategoryId.Should().Be(newCategoryId);
    }

    [Fact]
    public void UpdateLocation_Should_UpdateLatitudeAndLongitude() {
        // Arrange
        var camping = CreateSample();

        // Act
        camping.UpdateLocation(35.5m, -5.5m);

        // Assert
        camping.Latitude.Value.Should().Be(35.5m);
        camping.Longitude.Value.Should().Be(-5.5m);
    }

    [Fact]
    public void SetFacilities_Should_ReplaceAllFacilities() {
        // Arrange
        var camping = CreateSample();
        var f1 = Guid.NewGuid();
        var f2 = Guid.NewGuid();

        // Act
        camping.SetFacilities([f1, f2]);

        // Assert
        camping.FacilityIds.Should().BeEquivalentTo(new[] { f1, f2 });
    }

    [Fact]
    public void AddFacility_Should_AddFacilityId_WhenNotAlreadyPresent() {
        // Arrange
        var camping = CreateSample();
        var facilityId = Guid.NewGuid();

        // Act
        camping.AddFacility(facilityId);

        // Assert
        camping.FacilityIds.Should().Contain(facilityId);
    }

    [Fact]
    public void AddFacility_Should_NotDuplicate_WhenAlreadyPresent() {
        // Arrange
        var camping = CreateSample();
        var facilityId = Guid.NewGuid();
        camping.AddFacility(facilityId);

        // Act
        camping.AddFacility(facilityId);

        // Assert
        camping.FacilityIds.Should().HaveCount(1);
    }

    [Fact]
    public void AddFacility_Should_ThrowDomainException_WhenIdIsEmpty() {
        // Arrange
        var camping = CreateSample();

        // Act
        Action act = () => camping.AddFacility(Guid.Empty);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("FacilityId cannot be empty.");
    }

    [Fact]
    public void RemoveFacility_Should_RemoveFacilityId() {
        // Arrange
        var camping = CreateSample();
        var facilityId = Guid.NewGuid();
        camping.AddFacility(facilityId);

        // Act
        camping.RemoveFacility(facilityId);

        // Assert
        camping.FacilityIds.Should().NotContain(facilityId);
    }

    [Fact]
    public void AddCategory_Should_AddAdditionalCategoryId_WhenNotAlreadyPresent() {
        // Arrange
        var camping = CreateSample();
        var extraCategoryId = Guid.NewGuid();

        // Act
        camping.AddCategory(extraCategoryId);

        // Assert
        camping.AdditionalCategoryIds.Should().Contain(extraCategoryId);
    }

    [Fact]
    public void AddCategory_Should_ThrowDomainException_WhenIdIsEmpty() {
        // Arrange
        var camping = CreateSample();

        // Act
        Action act = () => camping.AddCategory(Guid.Empty);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("CategoryId cannot be empty.");
    }

    [Fact]
    public void RemoveCategory_Should_RemoveCategoryId() {
        // Arrange
        var camping = CreateSample();
        var extraCategoryId = Guid.NewGuid();
        camping.AddCategory(extraCategoryId);

        // Act
        camping.RemoveCategory(extraCategoryId);

        // Assert
        camping.AdditionalCategoryIds.Should().NotContain(extraCategoryId);
    }

    [Fact]
    public void SetDeleted_Should_SetDeletedOn() {
        // Arrange
        var camping = CreateSample();

        // Act
        camping.SetDeleted();

        // Assert
        camping.DeletedOn.Should().NotBeNull();
    }

    [Fact]
    public void Updated_Should_UpdateUpdatedOn() {
        // Arrange
        var camping = CreateSample();
        var before = camping.UpdatedOn.Value;

        // Act — pequeña espera para asegurar diferencia de timestamp
        System.Threading.Thread.Sleep(10);
        camping.Updated();

        // Assert
        camping.UpdatedOn.Value.Should().BeOnOrAfter(before);
    }
}
