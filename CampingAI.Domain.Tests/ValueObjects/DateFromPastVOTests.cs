using FluentAssertions;
using CampingAI.Domain.ValueObjects;

namespace CampingAI.Domain.Tests.ValueObjects;
public class DateFromPastVOTests {
    [Fact]
    public void Constructor_Should_CreateInstance_WhenDateIsInThePast() {//Happy path
        // Arrange
        var pastDate = DateTime.Now.AddDays(-1);

        // Act
        var vo = new DateFromPastVO(pastDate);

        // Assert
        vo.Value.Should().Be(pastDate);
    }

    [Fact]
    public void Constructor_Should_ThrowDomainException_WhenDateIsInTheFuture() {
        // Arrange
        var futureDate = DateTime.Now.AddSeconds(1);

        // Act
        Action act = () => new DateFromPastVO(futureDate);

        // Assert
        act.Should().Throw<Exceptions.DomainException>()
           .WithMessage("The date must be in the past");
    }

    [Fact]
    public void CreateNow_Should_CreateInstance_WithCurrentDateTime() {
        // Act
        var result = DateFromPastVO.CreateNow();

        // Assert
        result.Should().NotBeNull();
        result.Value.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMilliseconds(100));
    }
}