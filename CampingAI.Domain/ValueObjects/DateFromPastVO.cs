using CampingAI.Domain.Exceptions;

namespace CampingAI.Domain.ValueObjects;
public class DateFromPastVO {
    public DateTime Value { get; private set; }

    public DateFromPastVO(DateTime value) {
        Value = value;

        if (value >= DateTime.UtcNow)
            throw new DomainException("The date must be in the past");
    }

    public static DateFromPastVO CreateNow() {
        return new DateFromPastVO(DateTime.UtcNow.AddTicks(-1));
    }

    public override string ToString() {
        return Value.ToString();
    }

    public override bool Equals(object obj)
        => obj is DateFromPastVO other && Value == other.Value;

    public override int GetHashCode() => Value.GetHashCode();
}