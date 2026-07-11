using FluentAssertions;
using Moq;
using CampingAI.Application.Queries.Employee.GetEmployeeById;
using CampingAI.Application.Shared.Mappers;
using CampingAI.Domain.Repositories.Employees;

namespace CampingAI.Application.Tests.Queries;
public class GetEmployeeByIdQueryHandlerTests {
    private readonly Mock<IEmpoyeesReadRepository> _repositoryMock;
    private readonly Mock<GetEmployeeByIdItemDtoMapper> _mapperMock;
    private readonly GetEmployeeByIdQueryHandler _handler;

    public GetEmployeeByIdQueryHandlerTests() {
        _repositoryMock = new Mock<IEmpoyeesReadRepository>();
        _mapperMock = new Mock<GetEmployeeByIdItemDtoMapper>();
        _handler = new GetEmployeeByIdQueryHandler(_repositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowKeyNotFoundException_WhenEmployeeDoesNotExist() {
        // Arrange
        var employeeId = Guid.NewGuid();
        var query = new GetEmployeeByIdQuery(employeeId);

        _repositoryMock.Setup(r => r.Get(employeeId, true)).ReturnsAsync((Domain.Entities.Employee)null);

        // Act
        Func<Task> act = async () => await _handler.HandleAsync(query);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Employee with id {employeeId} not found.");
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnDto_WhenEmployeeExists() {
        // Arrange
        var employeeId = Guid.NewGuid();
        var query = new GetEmployeeByIdQuery(employeeId);

        var createdOn = DateTime.UtcNow.AddDays(-10);
        var updatedOn = DateTime.UtcNow.AddDays(-5);
        var lastLogin = DateTime.UtcNow.AddDays(-1);

        var employee = new Domain.Entities.Employee(
            employeeId,
            "john.doe",
            "John Doe",
            "john.doe@example.com",
            "123456789",
            "hashedPassword",
            1, 2, 3, 4,
            "987654321",
            lastLogin,
            createdOn,
            updatedOn,
            null);

        _repositoryMock.Setup(r => r.Get(employeeId, true))
            .ReturnsAsync(employee);

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(employeeId);
        result.Username.Should().Be(employee.Username);
        result.Name.Should().Be(employee.Name);
        result.Email.Should().Be(employee.Email);
        result.Fax.Should().Be(employee.Fax);
        result.CompanyId.Should().Be(employee.CompanyId);
        result.PortalId.Should().Be(employee.PortalId);
        result.RoleId.Should().Be(employee.RoleId);
        result.StatusId.Should().Be(employee.StatusId);
        result.Telephone.Should().Be(employee.Telephone);
        result.LastLogin.Should().Be(employee.LastLogin.Value);
        result.CreatedOn.Should().Be(employee.CreatedOn.Value);
        result.UpdatedOn.Should().Be(employee.UpdatedOn.Value);
        result.DeletedOn.Should().BeNull();

        _repositoryMock.Verify(r => r.Get(employeeId, true), Times.Once);
    }
}