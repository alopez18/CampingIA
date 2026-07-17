using FluentAssertions;
using CampingAI.Domain.Exceptions;
using CampingAI.Domain.ValueObjects;

namespace CampingAI.Domain.Tests.ValueObjects;
public class LongitudeVOTests {

    [Theory]
    [InlineData(0)]
    [InlineData(90)]
    [InlineData(-90)]
    [InlineData(180)]
    [InlineData(-180)]
    public void Constructor_Should_CreateInstance_WhenValueIsValid(decimal value) {
        // Act
        var vo = new LongitudeVO(value);

        // Assert
        vo.Value.Should().Be(value);
    }

    [Theory]
    [InlineData(180.001)]
    [InlineData(270)]
    [InlineData(181)]
    public void Constructor_Should_ThrowDomainException_WhenValueIsGreaterThan180(decimal value) {
        // Act
        Action act = () => new LongitudeVO(value);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Longitude must be between -180 and 180.");
    }

    [Theory]
    [InlineData(-180.001)]
    [InlineData(-270)]
    [InlineData(-181)]
    public void Constructor_Should_ThrowDomainException_WhenValueIsLessThanMinus180(decimal value) {
        // Act
        Action act = () => new LongitudeVO(value);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Longitude must be between -180 and 180.");
    }

    [Fact]
    public void Equals_Should_ReturnTrue_WhenValuesAreEqual() {
        // Arrange
        var vo1 = new LongitudeVO(-3.7m);
        var vo2 = new LongitudeVO(-3.7m);

        // Assert
        vo1.Equals(vo2).Should().BeTrue();
    }

    [Fact]
    public void Equals_Should_ReturnFalse_WhenValuesAreDifferent() {
        // Arrange
        var vo1 = new LongitudeVO(-3.7m);
        var vo2 = new LongitudeVO(2.1m);

        // Assert
        vo1.Equals(vo2).Should().BeFalse();
    }

    [Fact]
    public void ToString_Should_ReturnDecimalAsString() {
        // Arrange
        var vo = new LongitudeVO(-3.7m);

        // Assert
        vo.ToString().Should().Be((-3.7m).ToString());
    }
}
