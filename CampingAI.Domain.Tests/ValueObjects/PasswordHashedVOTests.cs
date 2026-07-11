using FluentAssertions;
using CampingAI.Domain.Exceptions;
using CampingAI.Domain.ValueObjects;

namespace CampingAI.Domain.Tests.ValueObjects;
public class PasswordHashedVOTests {
    [Theory]
    [InlineData("hash1234567890abcdef")]
    [InlineData("9f86d081884c7d659a2feaa0c55ad015a3bf4f1b2b0b822cd15d6c15b0f00a08")] // SHA256 válido
    [InlineData("salted-hash-xyz-123")]
    public void Constructor_Should_CreateInstance_WhenValueIsValid(string validHash) {
        // Act
        var vo = new PasswordHashedVO(validHash);

        // Assert
        vo.ToString().Should().Be(validHash);
        ((string)vo).Should().Be(validHash); // Conversión implícita a string
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_Should_ThrowDomainException_WhenValueIsNullOrWhitespace(string invalidHash) {
        // Act
        Action act = () => new PasswordHashedVO(invalidHash);

        // Assert
        act.Should().Throw<DomainException>()
           .WithMessage("The value cannot be null or empty");
    }

    [Fact]
    public void Equals_Should_ReturnTrue_WhenTwoHashesAreTheSame() {
        // Arrange
        var hash1 = new PasswordHashedVO("hash123");
        var hash2 = new PasswordHashedVO("hash123");

        // Act & Assert
        hash1.Equals(hash2).Should().BeTrue();
        (hash1 == hash2).Should().BeTrue();
        (hash1 != hash2).Should().BeFalse();
        hash1.GetHashCode().Should().Be(hash2.GetHashCode());
    }

    [Fact]
    public void Equals_Should_ReturnFalse_WhenTwoHashesAreDifferent() {
        // Arrange
        var hash1 = new PasswordHashedVO("hashABC");
        var hash2 = new PasswordHashedVO("hashXYZ");

        // Act & Assert
        hash1.Equals(hash2).Should().BeFalse();
        (hash1 == hash2).Should().BeFalse();
        (hash1 != hash2).Should().BeTrue();
    }

    [Fact]
    public void Equals_Should_ReturnFalse_WhenComparingWithNull() {
        // Arrange
        var hash = new PasswordHashedVO("hash123");

        // Act & Assert
        hash.Equals(null).Should().BeFalse();
        (hash == null).Should().BeFalse();
        (null == hash).Should().BeFalse();
        (hash != null).Should().BeTrue();
    }

    [Fact]
    public void Equals_Should_ReturnTrue_WhenBothAreNull() {
        // Arrange
        PasswordHashedVO hash1 = null;
        PasswordHashedVO hash2 = null;

        // Act & Assert
        (hash1 == hash2).Should().BeTrue();
        (hash1 != hash2).Should().BeFalse();
    }

    [Fact]
    public void ImplicitConversion_ShouldReturnString_WhenVOIsNotNull() {
        // Arrange
        var vo = new PasswordHashedVO("hash123");

        // Act
        string result = vo;

        // Assert
        result.Should().Be("hash123");
    }

    [Fact]
    public void ImplicitConversion_ShouldReturnNull_WhenVOIsNull() {
        // Arrange
        PasswordHashedVO vo = null;

        // Act
        string result = vo;

        // Assert
        result.Should().BeNull();
    }
}