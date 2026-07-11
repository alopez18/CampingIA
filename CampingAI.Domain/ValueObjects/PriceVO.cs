namespace CampingAI.Domain.ValueObjects;
public class PriceVO {
    public decimal Value { get; private set; }

    public PriceVO(decimal value) {
        if (value < 0m)
            throw new Exceptions.DomainException("Price cannot be negative.");
        Value = value;
    }

    public override string ToString() => Value.ToString();
    public override bool Equals(object obj) => obj is PriceVO other && Value == other.Value;
    public override int GetHashCode() => Value.GetHashCode();
}
