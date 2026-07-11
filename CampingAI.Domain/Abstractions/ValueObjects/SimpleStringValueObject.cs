namespace CampingAI.Domain.Abstractions.ValueObjects;

public abstract class SimpleStringValueObject<T>(string value) : IEquatable<T> where T : SimpleStringValueObject<T>
{
    private readonly string value = value;

    public override bool Equals(object obj) => Equals(obj as T);

    public override int GetHashCode()
    {
        return value.GetHashCode();
    }

    public bool Equals(T other)
    {
        if (other is null) return false;

        if (ReferenceEquals(this, other)) return true;

        return Equals(value, other.value);
    }

    public static bool operator ==(SimpleStringValueObject<T> x, SimpleStringValueObject<T> y)
    {
        if (ReferenceEquals(x, y))
        {
            return true;
        }

        if ((object)x == null || (object)y == null)
        {
            return false;
        }

        return x.Equals(y);
    }

    public static bool operator !=(SimpleStringValueObject<T> x, SimpleStringValueObject<T> y)
    {
        return !(x == y);

    }

    public static implicit operator string(SimpleStringValueObject<T> stringValue)
    {
        if (stringValue == null)
            return null;
        return stringValue.value;
    }

    public override string ToString()
    {
        if (value == null) return null;
        return value.ToString();
    }
}