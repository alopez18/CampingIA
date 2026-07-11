using FluentAssertions;
using CampingAI.Domain.Exceptions;
using CampingAI.Domain.ValueObjects;

namespace CampingAI.Domain.Tests.ValueObjects;
public class EmailVOTests {
    [Theory]
    [InlineData("user@example.com")]
    [InlineData("john.doe@domain.org")]
    [InlineData("my.email+alias@gmail.com")]
    [InlineData("contact@sub.domain.co.uk")]
    public void Constructor_Should_CreateInstance_WhenEmailIsValid(string validEmail) {
        // Act
        var emailVO = new EmailVO(validEmail);

        // Assert
        emailVO.ToString().Should().Be(validEmail);
        ((string)emailVO).Should().Be(validEmail); // Conversión implícita
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_Should_ThrowDomainException_WhenEmailIsNullOrWhitespace(string invalidEmail) {
        // Act
        Action act = () => new EmailVO(invalidEmail);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("The value cannot be null or empty");
    }

    [Theory]
    [InlineData("plainaddress")]
    [InlineData("missingatsign.com")]
    [InlineData("user@.com")]
    [InlineData("@domain.com")]
    [InlineData("user@domain")]
    [InlineData("user@domain,com")]
    public void Constructor_Should_ThrowDomainException_WhenEmailFormatIsInvalid(string invalidEmail) {
        // Act
        Action act = () => new EmailVO(invalidEmail);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Invalid email format");
    }

    [Fact]
    public void Equals_Should_ReturnTrue_WhenTwoEmailsAreTheSame() {
        // Arrange
        var email1 = new EmailVO("user@example.com");
        var email2 = new EmailVO("user@example.com");

        // Act & Assert
        email1.Equals(email2).Should().BeTrue();
        (email1 == email2).Should().BeTrue();
        (email1 != email2).Should().BeFalse();
        email1.GetHashCode().Should().Be(email2.GetHashCode());
    }

    [Fact]
    public void Equals_Should_ReturnFalse_WhenTwoEmailsAreDifferent() {
        // Arrange
        var email1 = new EmailVO("user1@example.com");
        var email2 = new EmailVO("user2@example.com");

        // Act & Assert
        email1.Equals(email2).Should().BeFalse();
        (email1 == email2).Should().BeFalse();
        (email1 != email2).Should().BeTrue();
    }

    [Fact]
    public void ImplicitConversion_ShouldReturnString_WhenVOIsNotNull() {
        // Arrange
        var emailVO = new EmailVO("test@example.com");

        // Act
        string result = emailVO;

        // Assert
        result.Should().Be("test@example.com");
    }

    [Fact]
    public void ImplicitConversion_ShouldReturnNull_WhenVOIsNull() {
        // Arrange
        EmailVO emailVO = null;

        // Act
        string result = emailVO;

        // Assert
        result.Should().BeNull();
    }
}