namespace CampingAI.Infra.Users.Mappers;

public class UsersMapper : Domain.Abstractions.Mappers.CompleteMapper<Models.CampingAI_DB.T_USERS, Domain.Entities.User>
{
    public override Domain.Entities.User Map(Models.CampingAI_DB.T_USERS src)
    {
        return new Domain.Entities.User(
            src.USR_IdUser,
            src.USR_Email,
            src.USR_PasswordHashed,
            src.USR_Name,
            src.USR_RoleId,
            src.USR_CreatedOn,
            src.USR_UpdatedOn,
            src.USR_DeletedOn);
    }

    public override Models.CampingAI_DB.T_USERS ReverseMap(Domain.Entities.User src)
    {
        return new Models.CampingAI_DB.T_USERS
        {
            USR_IdUser         = src.Id,
            USR_Email          = src.Email,
            USR_PasswordHashed = src.PasswordHashed,
            USR_Name           = src.Name,
            USR_RoleId         = src.RoleId,
            USR_CreatedOn      = src.CreatedOn.Value,
            USR_UpdatedOn      = src.UpdatedOn.Value,
            USR_DeletedOn      = src.DeletedOn
        };
    }
}
