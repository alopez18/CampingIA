namespace CampingAI.Infra.Tests.Employees;
public static class SamplesGenerator {
    public static Models.CAMPING_AI_DB.T_EMPLOYEES CreateSampleDbEmployee(Guid employeeId) {
        var lastLoginBase = DateTime.Now.AddDays(-1);
        var createdOnBase = DateTime.Now.AddDays(-30);
        var updatedOnBase = DateTime.Now.AddDays(-1);
        return new Models.CAMPING_AI_DB.T_EMPLOYEES {
            EMP_IdEmployee = employeeId,
            EMP_Username = "testuser",
            EMP_Name = "Test Employee",
            EMP_Email = "test@example.com",
            EMP_Telephone = "123456789",
            EMP_Fax = "987654321",
            EMP_Password = "hashedpassword123",
            EMP_CompanyId = 1,
            EMP_PortalId = 1,
            EMP_RoleId = 1,
            EMP_StatusId = 1,
            EMP_LastLogin = new DateTime(lastLoginBase.Year, lastLoginBase.Month, lastLoginBase.Day, lastLoginBase.Hour, lastLoginBase.Minute, 0, 0, 0, lastLoginBase.Kind),
            EMP_CreatedOn = new DateTime(createdOnBase.Year, createdOnBase.Month, createdOnBase.Day, createdOnBase.Hour, createdOnBase.Minute, 0, 0, 0, createdOnBase.Kind),
            EMP_UpdatedOn = new DateTime(updatedOnBase.Year, updatedOnBase.Month, updatedOnBase.Day, updatedOnBase.Hour, updatedOnBase.Minute, 0, 0, 0, updatedOnBase.Kind),
            EMP_DeletedOn = null
        };
    }

    public static Domain.Entities.Employee CreateSampleDomainEmployee(Guid employeeId) {
        var lastLoginBase = DateTime.Now.AddDays(-1);
        var createdOnBase = DateTime.Now.AddDays(-30);
        var updatedOnBase = DateTime.Now.AddDays(-1);
        return new Domain.Entities.Employee(
            idEmployee: employeeId,
            username: "testuser",
            name: "Test Employee",
            email: "test@example.com",
            fax: "987654321",
            passwordHashed: "hashedpassword123",
            companyId: 1,
            portalId: 1,
            roleId: 1,
            statusId: 1,
            telephone: "123456789",
            lastLogin: new DateTime(lastLoginBase.Year, lastLoginBase.Month, lastLoginBase.Day, lastLoginBase.Hour, lastLoginBase.Minute, 0, 0, 0, lastLoginBase.Kind),
            createdOn: new DateTime(createdOnBase.Year, createdOnBase.Month, createdOnBase.Day, createdOnBase.Hour, createdOnBase.Minute, 0, 0, 0, createdOnBase.Kind),
            updatedOn: new DateTime(updatedOnBase.Year, updatedOnBase.Month, updatedOnBase.Day, updatedOnBase.Hour, updatedOnBase.Minute, 0, 0, 0, updatedOnBase.Kind),
            deletedOn: null
        );
    }
}