using FluentAssertions;
using CampingAI.Domain.Exceptions;
using CampingAI.Domain.ValueObjects;

namespace CampingAI.Domain.Tests.ValueObjects;
public class ReservationDateVOTests {

    [Fact]
    public void Constructor_Should_CreateInstance_WhenDatesAreValid() {
        // Arrange
        var checkIn  = DateTime.Today.AddDays(1);
        var checkOut = DateTime.Today.AddDays(5);

        // Act
        var vo = new ReservationDateVO(checkIn, checkOut);

        // Assert
        vo.CheckIn.Should().Be(checkIn);
        vo.CheckOut.Should().Be(checkOut);
        vo.Nights.Should().Be(4);
    }

    [Fact]
    public void Constructor_Should_ThrowDomainException_WhenCheckInIsInPast() {
        // Arrange
        var checkIn  = DateTime.Today.AddDays(-1);
        var checkOut = DateTime.Today.AddDays(3);

        // Act
        Action act = () => new ReservationDateVO(checkIn, checkOut);

        // Assert
        act.Should().Throw<DomainException>();
    }

    [Theory]
    [InlineData(0)]   // same day
    [InlineData(-1)]  // checkOut before checkIn
    public void Constructor_Should_ThrowDomainException_WhenCheckInIsNotBeforeCheckOut(int offsetDays) {
        // Arrange
        var checkIn  = DateTime.Today.AddDays(2);
        var checkOut = checkIn.AddDays(offsetDays);

        // Act
        Action act = () => new ReservationDateVO(checkIn, checkOut);

        // Assert
        act.Should().Throw<DomainException>();
    }
}
