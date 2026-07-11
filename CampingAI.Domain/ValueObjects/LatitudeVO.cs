namespace CampingAI.Domain.ValueObjects;
public class LatitudeVO {
    public decimal Value { get; private set; }

    public LatitudeVO(decimal value) {
        if (value < -90m || value > 90m)
            throw new Exceptions.DomainException("Latitude must be between -90 and 90.");
        Value = value;
    }

    public override string ToString() => Value.ToString();
    public override bool Equals(object obj) => obj is LatitudeVO other && Value == other.Value;
    public override int GetHashCode() => Value.GetHashCode();
}
