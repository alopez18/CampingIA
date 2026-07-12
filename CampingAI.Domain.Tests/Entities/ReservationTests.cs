using FluentAssertions;
using CampingAI.Domain.Entities;
using CampingAI.Domain.Enums;
using CampingAI.Domain.Exceptions;

namespace CampingAI.Domain.Tests.Entities;
public class ReservationTests {

    private static Reservation BuildReservation(int statusId = (int)ReservationStatus.Pending) {
        return new Reservation(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.Today.AddDays(2),
            DateTime.Today.AddDays(5),
            100m,
            statusId,
            DateTime.Now,
            DateTime.Now,
            null);
    }

    [Fact]
    public void Cancel_Should_SetStatusToCancelled_WhenReservationIsPending() {
        // Arrange
        var reservation = BuildReservation((int)ReservationStatus.Pending);

        // Act
        reservation.Cancel();

        // Assert
        reservation.StatusId.Should().Be((int)ReservationStatus.Cancelled);
    }

    [Fact]
    public void Cancel_Should_ThrowDomainException_WhenAlreadyCancelled() {
        // Arrange
        var reservation = BuildReservation((int)ReservationStatus.Cancelled);

        // Act
        Action act = () => reservation.Cancel();

        // Assert
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void CreateNew_Should_SetStatusToPending() {
        // Arrange & Act
        var reservation = Reservation.CreateNew(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.Today.AddDays(1),
            DateTime.Today.AddDays(3),
            80m,
            (int)ReservationStatus.Pending);

        // Assert
        reservation.StatusId.Should().Be((int)ReservationStatus.Pending);
        reservation.Id.Should().NotBeEmpty();
    }
}
