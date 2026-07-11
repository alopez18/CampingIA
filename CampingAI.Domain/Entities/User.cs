namespace CampingAI.Domain.Entities;
public class User : Abstractions.Entities.Deleteable, Abstractions.Entities.IAuditableEntity {

    public ValueObjects.EmailVO Email { get; private set; }
    public ValueObjects.PasswordHashedVO PasswordHashed { get; private set; }
    public string? Name { get; private set; }
    public int RoleId { get; private set; }

    public ValueObjects.DateFromPastVO CreatedOn { get; set; }
    public ValueObjects.DateFromPastVO UpdatedOn { get; set; }

    public User(Guid idUser,
                string email,
                string passwordHashed,
                string? name,
                int roleId,
                DateTime createdOn,
                DateTime updatedOn,
                DateTime? deletedOn) : base(idUser, deletedOn) {
        if (idUser == Guid.Empty)
            throw new ArgumentException("The user ID cannot be empty.", nameof(idUser));

        Email = new ValueObjects.EmailVO(email);
        PasswordHashed = new ValueObjects.PasswordHashedVO(passwordHashed);
        Name = name;
        RoleId = roleId;
        CreatedOn = new(createdOn);
        UpdatedOn = new(updatedOn);
    }

    public static User CreateNew(string email,
                                 string passwordHashed,
                                 string? name,
                                 int roleId) {
        return new User(Guid.NewGuid(),
                        email,
                        passwordHashed,
                        name,
                        roleId,
                        DateTime.Now,
                        DateTime.Now,
                        null);
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

    public void Updated() {
        UpdatedOn = ValueObjects.DateFromPastVO.CreateNow();
    }

    public void Created() {
        CreatedOn = ValueObjects.DateFromPastVO.CreateNow();
        UpdatedOn = ValueObjects.DateFromPastVO.CreateNow();
    }
}
