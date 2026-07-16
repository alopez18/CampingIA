namespace CampingAI.Domain.ValueObjects;
public class RoleVO {
    public int Value { get; private set; }

    public RoleVO(int value) {
        if (!Enum.IsDefined(typeof(Enums.UserRole), value))
            throw new Exceptions.DomainException($"El rol '{value}' no es válido.");
        Value = value;
    }

    public RoleVO(Enums.UserRole role) : this((int)role) {
    }

    public Enums.UserRole Role => (Enums.UserRole)Value;
    public string Name => Role.ToString();

    public override string ToString() => Value.ToString();
    public override bool Equals(object obj) => obj is RoleVO other && Value == other.Value;
    public override int GetHashCode() => Value.GetHashCode();
}
