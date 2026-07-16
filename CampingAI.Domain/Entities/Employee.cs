namespace CampingAI.Domain.Entities;
public class Employee : Abstractions.Entities.Deleteable, Abstractions.Entities.IAuditableEntity {

    public ValueObjects.EmployeeUserNameVO Username { get; private set; }
    public string? Name { get; private set; }
    public ValueObjects.EmailVO Email { get; private set; }
    public string? Telephone { get; private set; }
    public string? Fax { get; private set; }

    public ValueObjects.PasswordHashedVO PasswordHashed { get; private set; }

    public int CompanyId { get; private set; }
    public int PortalId { get; private set; }
    public int RoleId { get; private set; }
    public int StatusId { get; private set; }


    public ValueObjects.DateFromPastVO? LastLogin { get; private set; }
    public ValueObjects.DateFromPastVO CreatedOn { get; set; }
    public ValueObjects.DateFromPastVO UpdatedOn { get; set; }


    public Employee(Guid idEmployee,
                    string username,
                    string? name,
                    string email,
                    string? fax,
                    string passwordHashed,
                    int companyId,
                    int portalId,
                    int roleId,
                    int statusId,
                    string? telephone,
                    DateTime? lastLogin,
                    DateTime createdOn,
                    DateTime updatedOn,
                    DateTime? deletedOn) : base(idEmployee, deletedOn) {
        if (idEmployee == Guid.Empty) {
            throw new ArgumentException("The employee ID cannot be empty.", nameof(idEmployee));
        }
        Username = new ValueObjects.EmployeeUserNameVO(username);
        Name = name;
        Email = new ValueObjects.EmailVO(email);
        Fax = fax;
        PasswordHashed = new ValueObjects.PasswordHashedVO(passwordHashed);
        CompanyId = companyId;
        PortalId = portalId;
        RoleId = roleId;
        StatusId = statusId;
        Telephone = telephone;
        LastLogin = lastLogin.HasValue ? new(lastLogin.Value) : null;
        CreatedOn = new(createdOn);
        UpdatedOn = new(updatedOn);
    }




    public static Employee CreateNew(string username,
                              string? name,
                              string email,
                              string? fax,
                              string passwordHashed,
                              int companyId,
                              int portalId,
                              int roleId,
                              int statusId,
                              string? telephone) {
        return new Employee(Guid.NewGuid(),
                            username,
                            name,
                            email,
                            fax,
                            passwordHashed,
                            companyId,
                            portalId,
                            roleId,
                            statusId,
                            telephone,
                            null,
                            DateTime.UtcNow,
                            DateTime.UtcNow,
                            null);
    }



    public void UpdateIdentifiers(int companyId,
                                  int portalId,
                                  int roleId,
                                  int statusId) {
        CompanyId = companyId;
        PortalId = portalId;
        RoleId = roleId;
        StatusId = statusId;
    }

    public void UpdateIdentity(string userName,
                               string? name,
                               string email,
                               string? fax,
                               string? telephone) {
        Username = new ValueObjects.EmployeeUserNameVO(userName);
        Name = name;
        Email = new ValueObjects.EmailVO(email);
        Fax = fax;
        Telephone = telephone;
    }

    public void UpdatePassword(string password) {
        PasswordHashed = new ValueObjects.PasswordHashedVO(password);
    }

    public void Updated() {
        this.UpdatedOn = Domain.ValueObjects.DateFromPastVO.CreateNow();
    }

    public void Created() {
        this.CreatedOn = Domain.ValueObjects.DateFromPastVO.CreateNow();
        this.UpdatedOn = Domain.ValueObjects.DateFromPastVO.CreateNow();
    }
}