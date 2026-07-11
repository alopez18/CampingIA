namespace CampingAI.WebApi.Controllers.api.RedArbor.DTO;
public class EmployeeItemResponseDto {
    public Guid Id { get; set; }
    public string Username { get; set; } = null!;
    public string? Name { get; set; }
    public string Email { get; set; } = null!;
    public string? Fax { get; set; }
    public int CompanyId { get; set; }
    public int PortalId { get; set; }
    public int RoleId { get; set; }
    public int StatusId { get; set; }
    public string? Telephone { get; set; }

    public DateTime? UpdatedOn { get; set; }
    public DateTime? CreatedOn { get; set; }
    public DateTime? LastLogin { get; set; }
    public DateTime? DeletedOn { get; set; }

}

