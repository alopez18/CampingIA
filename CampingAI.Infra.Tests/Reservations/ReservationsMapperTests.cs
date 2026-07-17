using FluentAssertions;
using CampingAI.Infra.Reservations.Mappers;
using CampingAI.Infra.Models.CampingAI_DB;
using CampingAI.Domain.Enums;

namespace CampingAI.Infra.Tests.Reservations;
public class ReservationsMapperTests {

    private readonly ReservationsMapper _mapper = new();

    private static T_RESERVATIONS CreateSampleDbReservation(Guid? id = null) {
        var now = DateTime.UtcNow.AddDays(-1);
        var checkIn = DateTime.UtcNow.Date.AddDays(1);
        var checkOut = DateTime.UtcNow.Date.AddDays(4);
        return new T_RESERVATIONS {
            RES_IdReservation = id ?? Guid.NewGuid(),
            RES_UserId        = Guid.NewGuid(),
            RES_CampingId     = Guid.NewGuid(),
            RES_CheckIn       = checkIn,
            RES_CheckOut      = checkOut,
            RES_TotalPrice    = 120m,
            RES_StatusId      = (int)ReservationStatus.Pending,
            RES_CreatedOn     = now,
            RES_UpdatedOn     = now,
            RES_DeletedOn     = null
        };
    }

    [Fact]
    public void Map_Should_MapDbModelToDomainEntity() {
        // Arrange
        var dbModel = CreateSampleDbReservation();

        // Act
        var entity = _mapper.Map(dbModel);

        // Assert
        entity.Should().NotBeNull();
        entity.Id.Should().Be(dbModel.RES_IdReservation);
        entity.UserId.Should().Be(dbModel.RES_UserId);
        entity.CampingId.Should().Be(dbModel.RES_CampingId);
        entity.Dates.CheckIn.Should().Be(dbModel.RES_CheckIn);
        entity.Dates.CheckOut.Should().Be(dbModel.RES_CheckOut);
        entity.TotalPrice.Value.Should().Be(dbModel.RES_TotalPrice);
        entity.StatusId.Should().Be(dbModel.RES_StatusId);
        entity.DeletedOn.Should().BeNull();
    }

    [Fact]
    public void Map_Should_MapDeletedOn_WhenProvided() {
        // Arrange
        var deletedOn = DateTime.UtcNow.AddHours(-1);
        var dbModel = CreateSampleDbReservation();
        dbModel.RES_DeletedOn = deletedOn;

        // Act
        var entity = _mapper.Map(dbModel);

        // Assert
        entity.DeletedOn.Should().Be(deletedOn);
    }

    [Fact]
    public void ReverseMap_Should_MapDomainEntityToDbModel() {
        // Arrange
        var checkIn  = DateTime.UtcNow.Date.AddDays(1);
        var checkOut = DateTime.UtcNow.Date.AddDays(5);
        var entity = Domain.Entities.Reservation.CreateNew(
            Guid.NewGuid(), Guid.NewGuid(), checkIn, checkOut, 200m, (int)Domain.Enums.ReservationStatus.Pending);

        // Act
        var dbModel = _mapper.ReverseMap(entity);

        // Assert
        dbModel.RES_IdReservation.Should().Be(entity.Id);
        dbModel.RES_UserId.Should().Be(entity.UserId);
        dbModel.RES_CampingId.Should().Be(entity.CampingId);
        dbModel.RES_CheckIn.Should().Be(entity.Dates.CheckIn);
        dbModel.RES_CheckOut.Should().Be(entity.Dates.CheckOut);
        dbModel.RES_TotalPrice.Should().Be(entity.TotalPrice.Value);
        dbModel.RES_StatusId.Should().Be(entity.StatusId);
        dbModel.RES_DeletedOn.Should().BeNull();
    }

    [Fact]
    public void Map_Should_MapCollection_WhenMultipleModelsProvided() {
        // Arrange
        var dbModels = new List<T_RESERVATIONS> {
            CreateSampleDbReservation(),
            CreateSampleDbReservation()
        };

        // Act
        var entities = _mapper.Map(dbModels).ToList();

        // Assert
        entities.Should().HaveCount(2);
    }
}
