using FluentAssertions;
using CampingAI.Domain.Entities;

namespace CampingAI.Domain.Tests.Entities;
public class EmployeeTests {
    [Fact]
    public void Constructor_ShouldInitializePropertiesCorrectly() {
        // Arrange
        var id = Guid.NewGuid();
        var username = "testuser";
        var name = "John Doe";
        var email = "john.doe@example.com";
        var fax = "123456789";
        var password = "hashedPassword123";
        var companyId = 1;
        var portalId = 2;
        var roleId = 3;
        var statusId = 4;
        var telephone = "987654321";
        var lastLogin = DateTime.UtcNow.AddDays(-1);
        var createdOn = DateTime.UtcNow.AddDays(-10);
        var updatedOn = DateTime.UtcNow.AddDays(-5);
        var deletedOn = DateTime.UtcNow;

        // Act
        var employee = new Employee(
            id,
            username,
            name,
            email,
            fax,
            password,
            companyId,
            portalId,
            roleId,
            statusId,
            telephone,
            lastLogin,
            createdOn,
            updatedOn,
            deletedOn
        );

        // Assert
        employee.Id.Should().Be(id);
        employee.Username.ToString().Should().Be(username.ToString());
        employee.Name.Should().Be(name);
        employee.Email.ToString().Should().Be(email);
        employee.Fax.Should().Be(fax);
        employee.PasswordHashed.ToString().Should().Be(password);
        employee.CompanyId.Should().Be(companyId);
        employee.PortalId.Should().Be(portalId);
        employee.RoleId.Should().Be(roleId);
        employee.StatusId.Should().Be(statusId);
        employee.Telephone.Should().Be(telephone);
        employee.LastLogin.Value.Should().BeCloseTo(lastLogin, TimeSpan.FromSeconds(1));
        employee.CreatedOn.Value.Should().BeCloseTo(createdOn, TimeSpan.FromSeconds(1));
        employee.UpdatedOn.Value.Should().BeCloseTo(updatedOn, TimeSpan.FromSeconds(1));
        employee.DeletedOn.Should().BeCloseTo(deletedOn, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void CreateNew_ShouldInitializeEmployeeWithDefaults() {
        // Act
        var employee = Employee.CreateNew(
            "newuser",
            "Jane Doe",
            "jane.doe@example.com",
            "111222333",
            "hashedPass",
            1,
            2,
            3,
            4,
            "444555666"
        );

        // Assert
        employee.Id.Should().NotBeEmpty();
        employee.Username.ToString().Should().Be("newuser");
        employee.Name.Should().Be("Jane Doe");
        employee.Email.ToString().Should().Be("jane.doe@example.com");
        employee.Fax.Should().Be("111222333");
        employee.PasswordHashed.ToString().Should().Be("hashedPass");
        employee.CompanyId.Should().Be(1);
        employee.PortalId.Should().Be(2);
        employee.RoleId.Should().Be(3);
        employee.StatusId.Should().Be(4);
        employee.Telephone.Should().Be("444555666");
        employee.LastLogin.Should().BeNull();
        employee.CreatedOn.Value.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        employee.UpdatedOn.Value.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        employee.DeletedOn.Should().BeNull();
    }

    [Fact]
    public void UpdateIdentifiers_ShouldUpdateAllIdentifiers() {
        // Arrange
        var employee = Employee.CreateNew("user", "Name", "email@test.com", "fax", "pass", 1, 2, 3, 4, "tel");

        // Act
        employee.UpdateIdentifiers(10, 20, 30, 40);

        // Assert
        employee.CompanyId.Should().Be(10);
        employee.PortalId.Should().Be(20);
        employee.RoleId.Should().Be(30);
        employee.StatusId.Should().Be(40);
    }

    [Fact]
    public void UpdateIdentity_ShouldUpdateAllIdentityFields() {
        // Arrange
        var employee = Employee.CreateNew("user", "Name", "email@test.com", "fax", "pass", 1, 2, 3, 4, "tel");

        // Act
        employee.UpdateIdentity("newuser", "New Name", "newemail@test.com", "newfax", "newtel");

        // Assert
        employee.Username.ToString().Should().Be("newuser");
        employee.Name.Should().Be("New Name");
        employee.Email.ToString().Should().Be("newemail@test.com");
        employee.Fax.Should().Be("newfax");
        employee.Telephone.Should().Be("newtel");
    }

    [Fact]
    public void UpdatePassword_ShouldChangePasswordHash() {
        // Arrange
        var employee = Employee.CreateNew("user", "Name", "email@test.com", "fax", "oldPass", 1, 2, 3, 4, "tel");

        // Act
        employee.UpdatePassword("newHashedPass");

        // Assert
        employee.PasswordHashed.ToString().Should().Be("newHashedPass");
    }

    [Fact]
    public void SetDeleted_ShouldSetDeletedOn() {
        // Arrange
        var employee = Employee.CreateNew("user", "Name", "email@test.com", "fax", "pass", 1, 2, 3, 4, "tel");

        // Act
        employee.SetDeleted();

        // Assert
        employee.DeletedOn.Should().NotBeNull();
        employee.DeletedOn.Value.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void SetUndeleted_ShouldClearDeletedOn() {
        // Arrange
        var employee = Employee.CreateNew("user", "Name", "email@test.com", "fax", "pass", 1, 2, 3, 4, "tel");
        employee.SetDeleted();

        // Act
        employee.SetUndeleted();

        // Assert
        employee.DeletedOn.Should().BeNull();
    }

    [Fact]
    public void Updated_ShouldUpdateUpdatedOn() {
        // Arrange
        var employee = Employee.CreateNew("user", "Name", "email@test.com", "fax", "pass", 1, 2, 3, 4, "tel");
        var previousUpdatedOn = employee.UpdatedOn.Value;

        // Act
        employee.Updated();

        // Assert
        employee.UpdatedOn.Value.Should().BeAfter(previousUpdatedOn);
    }

    [Fact]
    public void Created_ShouldInitializeCreatedOnAndUpdatedOn() {
        // Arrange
        var employee = Employee.CreateNew("user", "Name", "email@test.com", "fax", "pass", 1, 2, 3, 4, "tel");

        // Act
        employee.Created();

        // Assert
        employee.CreatedOn.Value.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        employee.UpdatedOn.Value.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }
}