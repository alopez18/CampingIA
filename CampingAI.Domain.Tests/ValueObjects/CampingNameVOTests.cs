using FluentAssertions;
using CampingAI.Domain.Exceptions;
using CampingAI.Domain.ValueObjects;

namespace CampingAI.Domain.Tests.ValueObjects;
public class CampingNameVOTests {

    [Theory]
    [InlineData("Camping El Bosque")]
    [InlineData("La Playa")]
    [InlineData("A")]
    public void Constructor_Should_CreateInstance_WhenValueIsValid(string value) {
        // Act
        var vo = new CampingNameVO(value);

        // Assert
        vo.ToString().Should().Be(value);
        ((string)vo).Should().Be(value);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_Should_ThrowDomainException_WhenValueIsNullOrWhitespace(string invalidValue) {
        // Act
        Action act = () => new CampingNameVO(invalidValue);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("The value cannot be null or empty");
    }

    [Fact]
    public void Equals_Should_ReturnTrue_WhenTwoNamesAreTheSame() {
        // Arrange
        var vo1 = new CampingNameVO("Camping Sol");
        var vo2 = new CampingNameVO("Camping Sol");

        // Assert
        vo1.Equals(vo2).Should().BeTrue();
    }

    [Fact]
    public void Equals_Should_ReturnFalse_WhenTwoNamesAreDifferent() {
        // Arrange
        var vo1 = new CampingNameVO("Camping Sol");
        var vo2 = new CampingNameVO("Camping Luna");

        // Assert
        vo1.Equals(vo2).Should().BeFalse();
    }
}
