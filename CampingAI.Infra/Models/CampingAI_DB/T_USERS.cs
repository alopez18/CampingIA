namespace CampingAI.Infra.Models.CampingAI_DB;

public partial class T_USERS
{
    public Guid USR_IdUser { get; set; }

    public string USR_Email { get; set; } = null!;

    public string USR_PasswordHashed { get; set; } = null!;

    public string? USR_Name { get; set; }

    public int USR_RoleId { get; set; }

    public int USR_ManagerStatus { get; set; }

    public DateTime USR_CreatedOn { get; set; }

    public DateTime USR_UpdatedOn { get; set; }

    public DateTime? USR_DeletedOn { get; set; }
}
