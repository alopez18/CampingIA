using FluentAssertions;
using CampingAI.Infra.Campings.Mappers;
using CampingAI.Infra.Models.CampingAI_DB;
using CampingAI.Domain.Enums;

namespace CampingAI.Infra.Tests.Campings;
public class CampingsMapperTests {

    private readonly CampingsMapper _mapper = new();

    private static T_CAMPINGS CreateSampleDbCamping(Guid? id = null, Guid? ownerId = null) {
        var now = DateTime.UtcNow.AddDays(-1);
        return new T_CAMPINGS {
            CMP_IdCamping    = id ?? Guid.NewGuid(),
            CMP_Name         = "Camping El Bosque",
            CMP_Description  = "Un camping en el bosque",
            CMP_Latitude     = 40.4m,
            CMP_Longitude    = -3.7m,
            CMP_PricePerNight = 25m,
            CMP_OwnerId      = ownerId ?? Guid.NewGuid(),
            CMP_CategoryId   = Guid.NewGuid(),
            CMP_ProvinciaId  = null,
            CMP_CreatedOn    = now,
            CMP_UpdatedOn    = now,
            CMP_DeletedOn    = null
        };
    }

    [Fact]
    public void Map_Should_MapDbModelToDomainEntity() {
        // Arrange
        var dbModel = CreateSampleDbCamping();

        // Act
        var entity = _mapper.Map(dbModel);

        // Assert
        entity.Should().NotBeNull();
        entity.Id.Should().Be(dbModel.CMP_IdCamping);
        entity.Name.ToString().Should().Be(dbModel.CMP_Name);
        entity.Description.ToString().Should().Be(dbModel.CMP_Description);
        entity.Latitude.Value.Should().Be(dbModel.CMP_Latitude);
        entity.Longitude.Value.Should().Be(dbModel.CMP_Longitude);
        entity.PricePerNight.Value.Should().Be(dbModel.CMP_PricePerNight);
        entity.OwnerId.Should().Be(dbModel.CMP_OwnerId);
        entity.CategoryId.Should().Be(dbModel.CMP_CategoryId);
        entity.ProvinciaId.Should().BeNull();
        entity.DeletedOn.Should().BeNull();
    }

    [Fact]
    public void Map_Should_MapDeletedOnAndProvinciaId_WhenProvided() {
        // Arrange
        var provinciaId = Guid.NewGuid();
        var deletedOn = DateTime.UtcNow.AddHours(-2);
        var dbModel = CreateSampleDbCamping();
        dbModel.CMP_ProvinciaId = provinciaId;
        dbModel.CMP_DeletedOn = deletedOn;

        // Act
        var entity = _mapper.Map(dbModel);

        // Assert
        entity.ProvinciaId.Should().Be(provinciaId);
        entity.DeletedOn.Should().Be(deletedOn);
    }

    [Fact]
    public void ReverseMap_Should_MapDomainEntityToDbModel() {
        // Arrange
        var entity = Domain.Entities.Camping.CreateNew(
            "Camping Sol", "Descripción", 41.0m, 2.0m, 30m, Guid.NewGuid(), Guid.NewGuid());

        // Act
        var dbModel = _mapper.ReverseMap(entity);

        // Assert
        dbModel.CMP_IdCamping.Should().Be(entity.Id);
        dbModel.CMP_Name.Should().Be(entity.Name.ToString());
        dbModel.CMP_Description.Should().Be(entity.Description.ToString());
        dbModel.CMP_Latitude.Should().Be(entity.Latitude.Value);
        dbModel.CMP_Longitude.Should().Be(entity.Longitude.Value);
        dbModel.CMP_PricePerNight.Should().Be(entity.PricePerNight.Value);
        dbModel.CMP_OwnerId.Should().Be(entity.OwnerId);
        dbModel.CMP_CategoryId.Should().Be(entity.CategoryId);
        dbModel.CMP_DeletedOn.Should().BeNull();
    }

    [Fact]
    public void Map_Should_MapCollection_WhenMultipleModelsProvided() {
        // Arrange
        var dbModels = new List<T_CAMPINGS> {
            CreateSampleDbCamping(),
            CreateSampleDbCamping()
        };

        // Act
        var entities = _mapper.Map(dbModels).ToList();

        // Assert
        entities.Should().HaveCount(2);
        entities.Should().AllSatisfy(e => e.Should().NotBeNull());
    }
}
