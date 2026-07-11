using FluentAssertions;
using Moq;
using CampingAI.Application.Queries.Employees.GetEmployees;
using CampingAI.Application.Shared.DTOs;
using CampingAI.Application.Shared.Mappers;
using CampingAI.Domain.Entities;
using CampingAI.Domain.Repositories.Employees;

namespace CampingAI.Application.Tests.Queries;
public class GetEmployeesQueryHandlerTests {
    private readonly Mock<IEmpoyeesReadRepository> _repositoryMock;
    private readonly GetEmployeeByIdItemDtoMapper _mapper;
    private readonly GetEmployeesQueryHandler _handler;

    public GetEmployeesQueryHandlerTests() {
        _repositoryMock = new Mock<IEmpoyeesReadRepository>();
        _mapper = new GetEmployeeByIdItemDtoMapper();
        _handler = new GetEmployeesQueryHandler(_repositoryMock.Object, _mapper);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnMappedResult_WhenEmployeesExist() {
        // Arrange
        var employees = new List<Employee>
        {
                new Employee(
                    Guid.NewGuid(),
                    "john.doe",
                    "John Doe",
                    "john.doe@example.com",
                    "123456789",
                    "hashedPass",
                    1, 2, 3, 4,
                    "987654321",
                    DateTime.UtcNow.AddDays(-1),
                    DateTime.UtcNow.AddDays(-10),
                    DateTime.UtcNow.AddDays(-5),
                    null
                ),
                new Employee(
                    Guid.NewGuid(),
                    "jane.doe",
                    "Jane Doe",
                    "jane.doe@example.com",
                    "987654321",
                    "hashedPass2",
                    5, 6, 7, 8,
                    "111222333",
                    null,
                    DateTime.UtcNow.AddDays(-8),
                    DateTime.UtcNow.AddDays(-3),
                    null
                )
            };

        var expectedDtos = new List<GetEmployeeByIdItemDto>
        {
                new GetEmployeeByIdItemDto(employees[0].Id, employees[0].Username, employees[0].Name,
                    employees[0].Email, employees[0].Fax, employees[0].CompanyId, employees[0].PortalId,
                    employees[0].RoleId, employees[0].StatusId, employees[0].Telephone,
                    employees[0].LastLogin?.Value, employees[0].CreatedOn.Value,
                    employees[0].UpdatedOn.Value, employees[0].DeletedOn),

                new GetEmployeeByIdItemDto(employees[1].Id, employees[1].Username, employees[1].Name,
                    employees[1].Email, employees[1].Fax, employees[1].CompanyId, employees[1].PortalId,
                    employees[1].RoleId, employees[1].StatusId, employees[1].Telephone,
                    employees[1].LastLogin?.Value, employees[1].CreatedOn.Value,
                    employees[1].UpdatedOn.Value, employees[1].DeletedOn)
            };

        _repositoryMock.Setup(r => r.GetAllAsync(true))
            .ReturnsAsync(employees);

        var query = new GetEmployeesQuery();

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedDtos);

        _repositoryMock.Verify(r => r.GetAllAsync(true), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnEmptyList_WhenNoEmployeesExist() {
        // Arrange
        var emptyEmployees = new List<Employee>();

        _repositoryMock.Setup(r => r.GetAllAsync(true)).ReturnsAsync(emptyEmployees);

        var query = new GetEmployeesQuery();

        // Act
        var result = await _handler.HandleAsync(query);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();

        _repositoryMock.Verify(r => r.GetAllAsync(true), Times.Once);
    }
}