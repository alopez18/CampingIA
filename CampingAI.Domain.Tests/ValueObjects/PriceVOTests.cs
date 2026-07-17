using FluentAssertions;
using CampingAI.Domain.Exceptions;
using CampingAI.Domain.ValueObjects;

namespace CampingAI.Domain.Tests.ValueObjects;
public class PriceVOTests {

    [Theory]
    [InlineData(0)]
    [InlineData(25.99)]
    [InlineData(1000)]
    [InlineData(0.01)]
    public void Constructor_Should_CreateInstance_WhenValueIsValid(decimal value) {
        // Act
        var vo = new PriceVO(value);

        // Assert
        vo.Value.Should().Be(value);
    }

    [Theory]
    [InlineData(-0.01)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Constructor_Should_ThrowDomainException_WhenValueIsNegative(decimal value) {
        // Act
        Action act = () => new PriceVO(value);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Price cannot be negative.");
    }

    [Fact]
    public void Constructor_Should_CreateInstance_WhenValueIsZero() {
        // Act
        var vo = new PriceVO(0m);

        // Assert
        vo.Value.Should().Be(0m);
    }

    [Fact]
    public void Equals_Should_ReturnTrue_WhenValuesAreEqual() {
        // Arrange
        var vo1 = new PriceVO(50m);
        var vo2 = new PriceVO(50m);

        // Assert
        vo1.Equals(vo2).Should().BeTrue();
    }

    [Fact]
    public void Equals_Should_ReturnFalse_WhenValuesAreDifferent() {
        // Arrange
        var vo1 = new PriceVO(50m);
        var vo2 = new PriceVO(99m);

        // Assert
        vo1.Equals(vo2).Should().BeFalse();
    }

    [Fact]
    public void ToString_Should_ReturnDecimalAsString() {
        // Arrange
        var vo = new PriceVO(29.99m);

        // Assert
        vo.ToString().Should().Be(29.99m.ToString());
    }
}
