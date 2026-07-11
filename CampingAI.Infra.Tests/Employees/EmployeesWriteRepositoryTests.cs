using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace CampingAI.Infra.Tests.Employees;
public class EmployeesWriteRepositoryTests {
    private static Infra.Models.CAMPING_AI_DB.CAMPINGAI_TTContext CreateDbContext(string dbName) {
        var options = new DbContextOptionsBuilder<Infra.Models.CAMPING_AI_DB.CAMPINGAI_TTContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .EnableSensitiveDataLogging()
            .Options;

        var ctx = new Infra.Models.CAMPING_AI_DB.CAMPINGAI_TTContext(options);
        return ctx;
    }



    private static (Infra.Employees.EmployeesWriteRepository sut,
                    Infra.Models.CAMPING_AI_DB.CAMPINGAI_TTContext ctx,
                    Mock<ILogger<Infra.Employees.EmployeesWriteRepository>> loggerMock,
                    Infra.Employees.Mappers.EmployeesMapper mapper)
        BuildSut(string dbName) {
        var ctx = CreateDbContext(dbName);

        var loggerMock = new Mock<ILogger<Infra.Employees.EmployeesWriteRepository>>();


        var mapper = new Infra.Employees.Mappers.EmployeesMapper();


        var sut = new Infra.Employees.EmployeesWriteRepository(ctx, loggerMock.Object, mapper);
        return (sut, ctx, loggerMock, mapper);
    }

    [Fact]
    public async Task GetById_ReturnsNull_WhenNotFound() {
        var (sut, _, _, _) = BuildSut(nameof(GetById_ReturnsNull_WhenNotFound));

        var result = await sut.GetById(Guid.NewGuid(), onlyNotDeleted: true);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetById_ReturnsEntity_WhenFound_OnlyNotDeleted_True() {
        var (sut, ctx, _, _) = BuildSut(nameof(GetById_ReturnsEntity_WhenFound_OnlyNotDeleted_True));

        //Un empleado sin borrar
        var idNotDeleted = Guid.NewGuid();
        var employeeDbNotDeleted = SamplesGenerator.CreateSampleDbEmployee(idNotDeleted);
        ctx.T_EMPLOYEES.Add(employeeDbNotDeleted);

        // Un empleado borrado
        var idDeleted = Guid.NewGuid();
        var employeeDbDeleted = SamplesGenerator.CreateSampleDbEmployee(idDeleted);
        employeeDbDeleted.EMP_DeletedOn = DateTime.Now;
        ctx.T_EMPLOYEES.Add(employeeDbDeleted);

        //Guardamos los cambios.
        await ctx.SaveChangesAsync();

        var resultNotDeleted = await sut.GetById(idNotDeleted, onlyNotDeleted: true);
        Assert.NotNull(resultNotDeleted);
        Assert.Equal(idNotDeleted, resultNotDeleted!.Id);

        var resultDeleted = await sut.GetById(idDeleted, onlyNotDeleted: true);
        Assert.Null(resultDeleted);
    }

    [Fact]
    public async Task GetById_IncludesDeleted_WhenOnlyNotDeleted_False() {
        var (sut, ctx, _, _) = BuildSut(nameof(GetById_IncludesDeleted_WhenOnlyNotDeleted_False));
        var idDeleted = Guid.NewGuid();
        var employeeDbDeleted = SamplesGenerator.CreateSampleDbEmployee(idDeleted);
        employeeDbDeleted.EMP_DeletedOn = DateTime.Now;

        ctx.T_EMPLOYEES.Add(employeeDbDeleted);
        await ctx.SaveChangesAsync();

        var result = await sut.GetById(idDeleted, onlyNotDeleted: false);

        Assert.NotNull(result);
        Assert.Equal(idDeleted, result!.Id);
    }

    [Fact]
    public async Task SaveAsync_Adds_WhenNotExists() {
        var (sut, ctx, _, _) = BuildSut(nameof(SaveAsync_Adds_WhenNotExists));
        var id = Guid.NewGuid();
        var employee = SamplesGenerator.CreateSampleDomainEmployee(id);

        await sut.SaveAsync(employee);
        await ctx.SaveChangesAsync();

        var fromDb = await ctx.T_EMPLOYEES.AsNoTracking().FirstOrDefaultAsync(e => e.EMP_IdEmployee == id);
        Assert.NotNull(fromDb);
        Assert.Equal(id, fromDb!.EMP_IdEmployee);
    }

    [Fact]
    public async Task SaveAsync_Updates_WhenExists() {
        var (sut, ctx, mapperLogger, mapper) = BuildSut(nameof(SaveAsync_Updates_WhenExists));
        var id = Guid.NewGuid();

        var employeeDB = SamplesGenerator.CreateSampleDbEmployee(id);


        // Insert previo
        ctx.T_EMPLOYEES.Add(employeeDB);
        await ctx.SaveChangesAsync();
        // Limpiamos el ChangeTracker para simular un contexto nuevo
        ctx.ChangeTracker.Clear();

        var entity = mapper.Map(employeeDB);
        entity.UpdatePassword("NewPassword123!"); // Cambiamos un campo para verificar el update)
        await sut.SaveAsync(entity);

        // Verificamos que el estado del entity en el ChangeTracker sea Modified
        var entry = ctx.ChangeTracker.Entries<Models.CAMPING_AI_DB.T_EMPLOYEES>()
            .FirstOrDefault(e => e.Entity.EMP_IdEmployee == id);

        Assert.NotNull(entry);
        Assert.Equal(EntityState.Modified, entry!.State);

        // Persistimos para asegurar que no rompe al guardar
        await ctx.SaveChangesAsync();

        var fromDb = await ctx.T_EMPLOYEES.AsNoTracking().FirstOrDefaultAsync(e => e.EMP_IdEmployee == id);
        Assert.NotNull(fromDb);
    }
}

