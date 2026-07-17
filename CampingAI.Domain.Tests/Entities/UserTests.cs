using FluentAssertions;
using CampingAI.Domain.Entities;
using CampingAI.Domain.Enums;

namespace CampingAI.Domain.Tests.Entities;
public class UserTests {

    private static User CreateSample(
        string email = "user@example.com",
        string passwordHashed = "hashed_password_123",
        string? name = "Test User",
        UserRole role = UserRole.Comun) {
        return User.CreateNew(email, passwordHashed, name, role);
    }

    [Fact]
    public void CreateNew_Should_InitializeUserWithCorrectValues() {
        // Arrange
        var email = "test@example.com";
        var passwordHashed = "hashedpwd";
        var name = "John Doe";

        // Act
        var user = User.CreateNew(email, passwordHashed, name, UserRole.Comun);

        // Assert
        user.Should().NotBeNull();
        user.Id.Should().NotBe(Guid.Empty);
        user.Email.ToString().Should().Be(email);
        user.PasswordHashed.ToString().Should().Be(passwordHashed);
        user.Name.Should().Be(name);
        user.RoleId.Should().Be((int)UserRole.Comun);
        user.ManagerStatus.Should().Be(ManagerApprovalStatus.None);
        user.DeletedOn.Should().BeNull();
    }

    [Fact]
    public void CreateNew_Should_AllowNullName() {
        // Act
        var user = User.CreateNew("user@example.com", "pwd", null, UserRole.Comun);

        // Assert
        user.Name.Should().BeNull();
    }

    [Fact]
    public void UpdateProfile_Should_UpdateName() {
        // Arrange
        var user = CreateSample();

        // Act
        user.UpdateProfile("New Name");

        // Assert
        user.Name.Should().Be("New Name");
    }

    [Fact]
    public void UpdateEmail_Should_UpdateEmailAddress() {
        // Arrange
        var user = CreateSample();

        // Act
        user.UpdateEmail("newemail@example.com");

        // Assert
        user.Email.ToString().Should().Be("newemail@example.com");
    }

    [Fact]
    public void UpdatePassword_Should_UpdatePasswordHash() {
        // Arrange
        var user = CreateSample();
        var newHash = "new_hashed_password";

        // Act
        user.UpdatePassword(newHash);

        // Assert
        user.PasswordHashed.ToString().Should().Be(newHash);
    }

    [Fact]
    public void UpdateRole_Should_ChangeRole() {
        // Arrange
        var user = CreateSample(role: UserRole.Comun);

        // Act
        user.UpdateRole(UserRole.Gestor);

        // Assert
        user.RoleId.Should().Be((int)UserRole.Gestor);
    }

    [Fact]
    public void RequestManagerRole_Should_SetStatusToPending() {
        // Arrange
        var user = CreateSample();

        // Act
        user.RequestManagerRole();

        // Assert
        user.ManagerStatus.Should().Be(ManagerApprovalStatus.Pending);
    }

    [Fact]
    public void RequestManagerRole_Should_DoNothing_WhenAlreadyApproved() {
        // Arrange
        var user = CreateSample();
        user.ApproveManagerRole();

        // Act
        user.RequestManagerRole();

        // Assert
        user.ManagerStatus.Should().Be(ManagerApprovalStatus.Approved);
    }

    [Fact]
    public void ApproveManagerRole_Should_SetStatusApprovedAndRoleGestor() {
        // Arrange
        var user = CreateSample();
        user.RequestManagerRole();

        // Act
        user.ApproveManagerRole();

        // Assert
        user.ManagerStatus.Should().Be(ManagerApprovalStatus.Approved);
        user.RoleId.Should().Be((int)UserRole.Gestor);
    }

    [Fact]
    public void RejectManagerRole_Should_SetStatusRejected() {
        // Arrange
        var user = CreateSample();
        user.RequestManagerRole();

        // Act
        user.RejectManagerRole();

        // Assert
        user.ManagerStatus.Should().Be(ManagerApprovalStatus.Rejected);
    }

    [Fact]
    public void GrantManagerRoleInstantly_Should_ApproveAndSetGestorRole() {
        // Arrange
        var user = CreateSample();

        // Act
        user.GrantManagerRoleInstantly();

        // Assert
        user.ManagerStatus.Should().Be(ManagerApprovalStatus.Approved);
        user.RoleId.Should().Be((int)UserRole.Gestor);
    }

    [Fact]
    public void SetDeleted_Should_SetDeletedOn() {
        // Arrange
        var user = CreateSample();

        // Act
        user.SetDeleted();

        // Assert
        user.DeletedOn.Should().NotBeNull();
    }
}
