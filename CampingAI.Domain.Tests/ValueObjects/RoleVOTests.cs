using FluentAssertions;
using CampingAI.Domain.Enums;
using CampingAI.Domain.Exceptions;
using CampingAI.Domain.ValueObjects;

namespace CampingAI.Domain.Tests.ValueObjects;
public class RoleVOTests {
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(99)]
    public void Constructor_Should_CreateInstance_WhenRoleIsValid(int validRole) {
        // Act
        var roleVO = new RoleVO(validRole);

        // Assert
        roleVO.Value.Should().Be(validRole);
        roleVO.Role.Should().Be((UserRole)validRole);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(4)]
    [InlineData(-1)]
    [InlineData(100)]
    public void Constructor_Should_ThrowDomainException_WhenRoleIsInvalid(int invalidRole) {
        // Act
        Action act = () => new RoleVO(invalidRole);

        // Assert
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Constructor_Should_CreateInstance_FromEnum() {
        // Act
        var roleVO = new RoleVO(UserRole.Gestor);

        // Assert
        roleVO.Value.Should().Be((int)UserRole.Gestor);
        roleVO.Name.Should().Be(nameof(UserRole.Gestor));
    }
}
