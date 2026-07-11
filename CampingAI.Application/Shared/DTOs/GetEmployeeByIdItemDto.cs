namespace CampingAI.Application.Shared.DTOs;
public record GetEmployeeByIdItemDto(Guid Id,
                    string Username,
                    string? Name,
                    string Email,
                    string? Fax,
                    int CompanyId,
                    int PortalId,
                    int RoleId,
                    int StatusId,
                    string? Telephone,
                    DateTime? LastLogin,
                    DateTime CreatedOn,
                    DateTime UpdatedOn,
                    DateTime? DeletedOn) {
}

