namespace CampingAI.Domain.Entities;
public class User : Abstractions.Entities.Deleteable, Abstractions.Entities.IAuditableEntity {

    public ValueObjects.EmailVO Email { get; private set; }
    public ValueObjects.PasswordHashedVO PasswordHashed { get; private set; }
    public string? Name { get; private set; }
    public ValueObjects.RoleVO Role { get; private set; }
    public int RoleId => Role.Value;
    public Enums.ManagerApprovalStatus ManagerStatus { get; private set; }

    public ValueObjects.DateFromPastVO CreatedOn { get; set; }
    public ValueObjects.DateFromPastVO UpdatedOn { get; set; }

    public User(Guid idUser,
                string email,
                string passwordHashed,
                string? name,
                int roleId,
                DateTime createdOn,
                DateTime updatedOn,
                DateTime? deletedOn,
                Enums.ManagerApprovalStatus managerStatus = Enums.ManagerApprovalStatus.None) : base(idUser, deletedOn) {
        if (idUser == Guid.Empty)
            throw new ArgumentException("The user ID cannot be empty.", nameof(idUser));

        Email = new ValueObjects.EmailVO(email);
        PasswordHashed = new ValueObjects.PasswordHashedVO(passwordHashed);
        Name = name;
        Role = new ValueObjects.RoleVO(roleId);
        ManagerStatus = managerStatus;
        CreatedOn = new(createdOn);
        UpdatedOn = new(updatedOn);
    }

    public static User CreateNew(string email,
                                 string passwordHashed,
                                 string? name,
                                 Enums.UserRole role) {
        return new User(Guid.NewGuid(),
                        email,
                        passwordHashed,
                        name,
                        (int)role,
                        DateTime.Now,
                        DateTime.Now,
                        null,
                        Enums.ManagerApprovalStatus.None);
    }

    public void UpdateProfile(string? name) {
        Name = name;
    }

    public void UpdateEmail(string email) {
        Email = new ValueObjects.EmailVO(email);
    }

    public void UpdatePassword(string passwordHashed) {
        PasswordHashed = new ValueObjects.PasswordHashedVO(passwordHashed);
    }

    public void UpdateRole(Enums.UserRole role) {
        Role = new ValueObjects.RoleVO(role);
    }

    public void RequestManagerRole() {
        if (ManagerStatus == Enums.ManagerApprovalStatus.Approved)
            return;
        ManagerStatus = Enums.ManagerApprovalStatus.Pending;
    }

    public void ApproveManagerRole() {
        ManagerStatus = Enums.ManagerApprovalStatus.Approved;
        UpdateRole(Enums.UserRole.Gestor);
    }

    public void RejectManagerRole() {
        ManagerStatus = Enums.ManagerApprovalStatus.Rejected;
    }

    public void GrantManagerRoleInstantly() {
        ManagerStatus = Enums.ManagerApprovalStatus.Approved;
        UpdateRole(Enums.UserRole.Gestor);
    }

    public void Updated() {
        UpdatedOn = ValueObjects.DateFromPastVO.CreateNow();
    }

    public void Created() {
        CreatedOn = ValueObjects.DateFromPastVO.CreateNow();
        UpdatedOn = ValueObjects.DateFromPastVO.CreateNow();
    }
}
