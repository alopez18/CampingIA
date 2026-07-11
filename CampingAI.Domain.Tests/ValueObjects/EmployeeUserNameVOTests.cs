using FluentAssertions;
using CampingAI.Domain.Exceptions;
using CampingAI.Domain.ValueObjects;

namespace CampingAI.Domain.Tests.ValueObjects;

public class EmployeeUserNameVOTests {
    [Theory]
    [InlineData("JohnDoe")]
    [InlineData("Jane_Doe123")]
    [InlineData("User.Name")]
    public void Constructor_Should_CreateInstance_WhenValueIsValid(string validUserName) {
        // Act
        var vo = new EmployeeUserNameVO(validUserName);

        // Assert
        vo.ToString().Should().Be(validUserName);
        ((string)vo).Should().Be(validUserName); // Conversión implícita
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_Should_ThrowDomainException_WhenValueIsNullOrWhitespace(string invalidUserName) {
        // Act
        Action act = () => new EmployeeUserNameVO(invalidUserName);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("The value cannot be null or empty");
    }

    [Fact]
    public void Equals_Should_ReturnTrue_WhenTwoUserNamesAreTheSame() {
        // Arrange
        var userName1 = new EmployeeUserNameVO("JohnDoe");
        var userName2 = new EmployeeUserNameVO("JohnDoe");

        // Act & Assert
        userName1.Equals(userName2).Should().BeTrue();
        (userName1 == userName2).Should().BeTrue();
        (userName1 != userName2).Should().BeFalse();
        userName1.GetHashCode().Should().Be(userName2.GetHashCode());
    }

    [Fact]
    public void Equals_Should_ReturnFalse_WhenTwoUserNamesAreDifferent() {
        // Arrange
        var userName1 = new EmployeeUserNameVO("JohnDoe");
        var userName2 = new EmployeeUserNameVO("JaneDoe");

        // Act & Assert
        userName1.Equals(userName2).Should().BeFalse();
        (userName1 == userName2).Should().BeFalse();
        (userName1 != userName2).Should().BeTrue();
    }

    [Fact]
    public void Equals_Should_ReturnFalse_WhenComparingWithNull() {
        // Arrange
        var userName = new EmployeeUserNameVO("JohnDoe");

        // Act & Assert
        userName.Equals(null).Should().BeFalse();
        (userName == null).Should().BeFalse();
        (null == userName).Should().BeFalse();
        (userName != null).Should().BeTrue();
    }

    [Fact]
    public void Equals_Should_ReturnTrue_WhenBothAreNull() {
        // Arrange
        EmployeeUserNameVO userName1 = null;
        EmployeeUserNameVO userName2 = null;

        // Act & Assert
        (userName1 == userName2).Should().BeTrue();
        (userName1 != userName2).Should().BeFalse();
    }

    [Fact]
    public void ImplicitConversion_ShouldReturnString_WhenVOIsNotNull() {
        // Arrange
        var vo = new EmployeeUserNameVO("JohnDoe");

        // Act
        string result = vo;

        // Assert
        result.Should().Be("JohnDoe");
    }

    [Fact]
    public void ImplicitConversion_ShouldReturnNull_WhenVOIsNull() {
        // Arrange
        EmployeeUserNameVO vo = null;

        // Act
        string result = vo;

        // Assert
        result.Should().BeNull();
    }
}