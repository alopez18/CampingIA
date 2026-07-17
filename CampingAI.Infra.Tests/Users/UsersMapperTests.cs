using FluentAssertions;
using CampingAI.Infra.Users.Mappers;
using CampingAI.Infra.Models.CampingAI_DB;
using CampingAI.Domain.Enums;

namespace CampingAI.Infra.Tests.Users;
public class UsersMapperTests {

    private readonly UsersMapper _mapper = new();

    private static T_USERS CreateSampleDbUser(Guid? id = null) {
        var now = DateTime.UtcNow.AddDays(-1);
        return new T_USERS {
            USR_IdUser         = id ?? Guid.NewGuid(),
            USR_Email          = "user@example.com",
            USR_PasswordHashed = "hashed_password",
            USR_Name           = "Test User",
            USR_RoleId         = (int)UserRole.Comun,
            USR_ManagerStatus  = (int)ManagerApprovalStatus.None,
            USR_CreatedOn      = now,
            USR_UpdatedOn      = now,
            USR_DeletedOn      = null
        };
    }

    [Fact]
    public void Map_Should_MapDbModelToDomainEntity() {
        // Arrange
        var dbModel = CreateSampleDbUser();

        // Act
        var entity = _mapper.Map(dbModel);

        // Assert
        entity.Should().NotBeNull();
        entity.Id.Should().Be(dbModel.USR_IdUser);
        entity.Email.ToString().Should().Be(dbModel.USR_Email);
        entity.PasswordHashed.ToString().Should().Be(dbModel.USR_PasswordHashed);
        entity.Name.Should().Be(dbModel.USR_Name);
        entity.RoleId.Should().Be(dbModel.USR_RoleId);
        entity.ManagerStatus.Should().Be(ManagerApprovalStatus.None);
        entity.DeletedOn.Should().BeNull();
    }

    [Fact]
    public void Map_Should_MapDeletedOn_WhenProvided() {
        // Arrange
        var deletedOn = DateTime.UtcNow.AddHours(-1);
        var dbModel = CreateSampleDbUser();
        dbModel.USR_DeletedOn = deletedOn;

        // Act
        var entity = _mapper.Map(dbModel);

        // Assert
        entity.DeletedOn.Should().Be(deletedOn);
    }

    [Fact]
    public void Map_Should_MapManagerStatus_Correctly() {
        // Arrange
        var dbModel = CreateSampleDbUser();
        dbModel.USR_ManagerStatus = (int)ManagerApprovalStatus.Approved;
        dbModel.USR_RoleId = (int)UserRole.Gestor;

        // Act
        var entity = _mapper.Map(dbModel);

        // Assert
        entity.ManagerStatus.Should().Be(ManagerApprovalStatus.Approved);
        entity.RoleId.Should().Be((int)UserRole.Gestor);
    }

    [Fact]
    public void ReverseMap_Should_MapDomainEntityToDbModel() {
        // Arrange
        var entity = Domain.Entities.User.CreateNew("user@example.com", "hashed", "John", UserRole.Comun);

        // Act
        var dbModel = _mapper.ReverseMap(entity);

        // Assert
        dbModel.USR_IdUser.Should().Be(entity.Id);
        dbModel.USR_Email.Should().Be(entity.Email.ToString());
        dbModel.USR_PasswordHashed.Should().Be(entity.PasswordHashed.ToString());
        dbModel.USR_Name.Should().Be(entity.Name);
        dbModel.USR_RoleId.Should().Be(entity.RoleId);
        dbModel.USR_ManagerStatus.Should().Be((int)entity.ManagerStatus);
        dbModel.USR_DeletedOn.Should().BeNull();
    }
}
