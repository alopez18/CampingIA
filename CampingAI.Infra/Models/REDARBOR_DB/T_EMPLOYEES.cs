namespace CampingAI.Infra.Models.REDARBOR_DB;

public partial class T_EMPLOYEES {
    public Guid EMP_IdEmployee { get; set; }

    public string EMP_Username { get; set; } = null!;

    public string? EMP_Name { get; set; }

    public string EMP_Email { get; set; } = null!;

    public string? EMP_Fax { get; set; }

    public string EMP_Password { get; set; } = null!;

    public int EMP_CompanyId { get; set; }

    public int EMP_PortalId { get; set; }

    public int EMP_RoleId { get; set; }

    public int EMP_StatusId { get; set; }

    public string? EMP_Telephone { get; set; }

    public DateTime? EMP_LastLogin { get; set; }

    public DateTime EMP_CreatedOn { get; set; }

    public DateTime EMP_UpdatedOn { get; set; }

    public DateTime? EMP_DeletedOn { get; set; }
}
