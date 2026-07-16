using FluentAssertions;
using Moq;
using CampingAI.Application.Queries.Reservation.GetUserReservations;
using CampingAI.Domain.Repositories;

namespace CampingAI.Application.Tests.Queries.Reservation;
public class GetUserReservationsQueryHandlerTests {

    private readonly Mock<IReservationsReadRepository> _readRepositoryMock;
    private readonly GetUserReservationsQueryHandler _handler;

    public GetUserReservationsQueryHandlerTests() {
        _readRepositoryMock = new Mock<IReservationsReadRepository>();
        _handler = new GetUserReservationsQueryHandler(_readRepositoryMock.Object);
    }

    [Fact]
    public async Task HandleAsync_Should_ReturnReservations_ForGivenUser() {
        // Arrange
        var userId = Guid.NewGuid();
        var reservations = new List<Domain.Entities.Reservation> {
            new(Guid.NewGuid(), userId, Guid.NewGuid(),
                DateTime.UtcNow.Date.AddDays(1), DateTime.UtcNow.Date.AddDays(3),
                80m, (int)Domain.Enums.ReservationStatus.Pending,
                DateTime.UtcNow, DateTime.UtcNow, null)
        };

        _readRepositoryMock.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync(reservations);

        // Act
        var result = await _handler.HandleAsync(new GetUserReservationsQuery(userId));

        // Assert
        result.Should().HaveCount(1);
        result.First().UserId.Should().Be(userId);
    }

    [Fact]
    public async Task HandleAsync_Should_ReturnEmpty_WhenUserHasNoReservations() {
        // Arrange
        var userId = Guid.NewGuid();
        _readRepositoryMock.Setup(r => r.GetByUserIdAsync(userId)).ReturnsAsync([]);

        // Act
        var result = await _handler.HandleAsync(new GetUserReservationsQuery(userId));

        // Assert
        result.Should().BeEmpty();
    }
}
