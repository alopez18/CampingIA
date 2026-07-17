using FluentAssertions;
using CampingAI.Domain.Exceptions;
using CampingAI.Domain.ValueObjects;

namespace CampingAI.Domain.Tests.ValueObjects;
public class LatitudeVOTests {

    [Theory]
    [InlineData(0)]
    [InlineData(45.5)]
    [InlineData(-45.5)]
    [InlineData(90)]
    [InlineData(-90)]
    public void Constructor_Should_CreateInstance_WhenValueIsValid(decimal value) {
        // Act
        var vo = new LatitudeVO(value);

        // Assert
        vo.Value.Should().Be(value);
    }

    [Theory]
    [InlineData(90.001)]
    [InlineData(180)]
    [InlineData(91)]
    public void Constructor_Should_ThrowDomainException_WhenValueIsGreaterThan90(decimal value) {
        // Act
        Action act = () => new LatitudeVO(value);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Latitude must be between -90 and 90.");
    }

    [Theory]
    [InlineData(-90.001)]
    [InlineData(-180)]
    [InlineData(-91)]
    public void Constructor_Should_ThrowDomainException_WhenValueIsLessThanMinus90(decimal value) {
        // Act
        Action act = () => new LatitudeVO(value);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Latitude must be between -90 and 90.");
    }

    [Fact]
    public void Equals_Should_ReturnTrue_WhenValuesAreEqual() {
        // Arrange
        var vo1 = new LatitudeVO(40.5m);
        var vo2 = new LatitudeVO(40.5m);

        // Assert
        vo1.Equals(vo2).Should().BeTrue();
    }

    [Fact]
    public void Equals_Should_ReturnFalse_WhenValuesAreDifferent() {
        // Arrange
        var vo1 = new LatitudeVO(40.5m);
        var vo2 = new LatitudeVO(41.0m);

        // Assert
        vo1.Equals(vo2).Should().BeFalse();
    }

    [Fact]
    public void ToString_Should_ReturnDecimalAsString() {
        // Arrange
        var vo = new LatitudeVO(40.5m);

        // Assert
        vo.ToString().Should().Be(40.5m.ToString());
    }
}
