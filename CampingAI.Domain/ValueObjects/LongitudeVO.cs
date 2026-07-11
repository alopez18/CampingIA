namespace CampingAI.Domain.ValueObjects;
public class LongitudeVO {
    public decimal Value { get; private set; }

    public LongitudeVO(decimal value) {
        if (value < -180m || value > 180m)
            throw new Exceptions.DomainException("Longitude must be between -180 and 180.");
        Value = value;
    }

    public override string ToString() => Value.ToString();
    public override bool Equals(object obj) => obj is LongitudeVO other && Value == other.Value;
    public override int GetHashCode() => Value.GetHashCode();
}
