namespace CampingAI.Infra.Employees.Mappers {
    public class EmployeesMapper : Domain.Abstractions.Mappers.CompleteMapper<Models.CAMPING_AI_DB.T_EMPLOYEES, Domain.Entities.Employee> {
        public override Domain.Entities.Employee Map(Models.CAMPING_AI_DB.T_EMPLOYEES src) {
            var dst = new Domain.Entities.Employee(src.EMP_IdEmployee,
                                                    src.EMP_Username,
                                                    src.EMP_Name,
                                                    src.EMP_Email,
                                                    src.EMP_Fax,
                                                    src.EMP_Password,
                                                    src.EMP_CompanyId,
                                                    src.EMP_PortalId,
                                                    src.EMP_RoleId,
                                                    src.EMP_StatusId,
                                                    src.EMP_Telephone,
                                                    src.EMP_LastLogin,
                                                    src.EMP_CreatedOn,
                                                    src.EMP_UpdatedOn,
                                                    src.EMP_DeletedOn);
            return dst;
        }

        public override Models.CAMPING_AI_DB.T_EMPLOYEES ReverseMap(Domain.Entities.Employee src) {
            var dst = new Models.CAMPING_AI_DB.T_EMPLOYEES {
                EMP_IdEmployee = src.Id,
                EMP_Username = src.Username,
                EMP_Name = src.Name,
                EMP_Email = src.Email,
                EMP_Fax = src.Fax,
                EMP_Password = src.PasswordHashed,
                EMP_CompanyId = src.CompanyId,
                EMP_PortalId = src.PortalId,
                EMP_RoleId = src.RoleId,
                EMP_StatusId = src.StatusId,
                EMP_Telephone = src.Telephone,
                EMP_LastLogin = src.LastLogin != null ? src.LastLogin.Value : null,
                EMP_CreatedOn = src.CreatedOn.Value,
                EMP_UpdatedOn = src.UpdatedOn.Value,
                EMP_DeletedOn = src.DeletedOn
            };
            return dst;
        }
    }
}
