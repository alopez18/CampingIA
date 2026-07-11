using Dapper;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Dapper;
using CampingAI.Domain.Entities;
using CampingAI.Infra.Abstractions;
using CampingAI.Infra.Configuration.Factories.Interfaces;
using CampingAI.Infra.Employees;
using CampingAI.Infra.Employees.Mappers;
using System.Data;

namespace CampingAI.Infra.Tests.Employees;
public class EmployeesReadRepositoryTests {
    private readonly ModelExtractor<Models.REDARBOR_DB.T_EMPLOYEES> _modelExtractor;
    private readonly Mock<ISqlConnectionFactory> _sqlConnectionFactoryMock;
    private readonly EmployeesMapper _employeesMapper;
    private readonly Mock<ILogger<EmployeesReadRepository>> _loggerMock;
    private readonly Mock<IDbConnection> _dbConnectionMock;
    private readonly EmployeesReadRepository _repository;

    public EmployeesReadRepositoryTests() {
        _modelExtractor = new ModelExtractor<Models.REDARBOR_DB.T_EMPLOYEES>();
        _sqlConnectionFactoryMock = new Mock<ISqlConnectionFactory>();
        _employeesMapper = new EmployeesMapper();
        _loggerMock = new Mock<ILogger<EmployeesReadRepository>>();
        _dbConnectionMock = new Mock<IDbConnection>();

        // Configuramos la factory para devolver la conexión simulada
        _sqlConnectionFactoryMock
            .Setup(f => f.CreateConnection())
            .Returns(_dbConnectionMock.Object);

        // Inicializamos el repositorio
        _repository = new EmployeesReadRepository(
            _modelExtractor,
            _sqlConnectionFactoryMock.Object,
            _employeesMapper,
            _loggerMock.Object
        );
    }





    [Fact]
    public async Task Get_ShouldReturnEmployee_WhenFound() {
        // Arrange
        var employeeId = Guid.NewGuid();
        var dbEmployee = SamplesGenerator.CreateSampleDbEmployee(employeeId);

        // Mock connection
        var dbConnectionMock = new Mock<IDbConnection>();
        dbConnectionMock.SetupDapperAsync(c =>
            c.QueryFirstOrDefaultAsync<Models.REDARBOR_DB.T_EMPLOYEES>(
                It.IsAny<string>(),
                It.IsAny<object>(),
                null, null, null))
            .ReturnsAsync(dbEmployee);

        _sqlConnectionFactoryMock.Setup(f => f.CreateConnection())
            .Returns(dbConnectionMock.Object);



        var repository = new EmployeesReadRepository(
            _modelExtractor,
            _sqlConnectionFactoryMock.Object,
            _employeesMapper,
            _loggerMock.Object
        );

        // Act
        Employee? result = await repository.Get(employeeId, true);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(employeeId, result.Id);
        Assert.Equal(dbEmployee.EMP_Name, result.Name);
        Assert.Equal(dbEmployee.EMP_Telephone, result.Telephone);
        Assert.Equal(dbEmployee.EMP_Email, result.Email);
        Assert.Equal(dbEmployee.EMP_Fax, result.Fax);
        Assert.Equal(dbEmployee.EMP_CompanyId, result.CompanyId);
        Assert.Equal(dbEmployee.EMP_PortalId, result.PortalId);
        Assert.Equal(dbEmployee.EMP_RoleId, result.RoleId);
        Assert.Equal(dbEmployee.EMP_StatusId, result.StatusId);
        Assert.Equal(dbEmployee.EMP_LastLogin.Value, result.LastLogin.Value);
        Assert.Equal(dbEmployee.EMP_CreatedOn, result.CreatedOn.Value);
        Assert.Equal(dbEmployee.EMP_UpdatedOn, result.UpdatedOn.Value);
        Assert.Null(result.DeletedOn);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnMappedEmployees() {
        var employee1Id = Guid.NewGuid();
        var employee2Id = Guid.NewGuid();
        // Arrange
        var dbEmployees = new List<Models.REDARBOR_DB.T_EMPLOYEES>
        {
            SamplesGenerator.CreateSampleDbEmployee(employee1Id),
            SamplesGenerator.CreateSampleDbEmployee(employee2Id)
        };

        // Mock connection
        var dbConnectionMock = new Mock<IDbConnection>();
        dbConnectionMock.SetupDapperAsync(c =>
            c.QueryAsync<Models.REDARBOR_DB.T_EMPLOYEES>(
                It.IsAny<string>(),
                null, null, null, null))
            .ReturnsAsync(dbEmployees);

        _sqlConnectionFactoryMock.Setup(f => f.CreateConnection())
            .Returns(dbConnectionMock.Object);



        var repository = new EmployeesReadRepository(
            _modelExtractor,
            _sqlConnectionFactoryMock.Object,
            _employeesMapper,
            _loggerMock.Object
        );

        // Act
        IEnumerable<Employee> result = await repository.GetAllAsync(true);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());



        var resultList = result.ToList();
        Assert.Equal(dbEmployees[0].EMP_IdEmployee, resultList[0].Id);
        Assert.Equal(dbEmployees[0].EMP_Name, resultList[0].Name);
        Assert.Equal(dbEmployees[0].EMP_Telephone, resultList[0].Telephone);
        Assert.Equal(dbEmployees[0].EMP_Email, resultList[0].Email);
        Assert.Equal(dbEmployees[0].EMP_Fax, resultList[0].Fax);
        Assert.Equal(dbEmployees[0].EMP_CompanyId, resultList[0].CompanyId);
        Assert.Equal(dbEmployees[0].EMP_PortalId, resultList[0].PortalId);
        Assert.Equal(dbEmployees[0].EMP_RoleId, resultList[0].RoleId);
        Assert.Equal(dbEmployees[0].EMP_StatusId, resultList[0].StatusId);
        Assert.Equal(dbEmployees[0].EMP_LastLogin.Value, resultList[0].LastLogin.Value);
        Assert.Equal(dbEmployees[0].EMP_CreatedOn, resultList[0].CreatedOn.Value);
        Assert.Equal(dbEmployees[0].EMP_UpdatedOn, resultList[0].UpdatedOn.Value);
        Assert.Null(resultList[0].DeletedOn);
    }
}