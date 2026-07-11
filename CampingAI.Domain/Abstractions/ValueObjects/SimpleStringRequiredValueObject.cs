namespace CampingAI.Domain.Abstractions.ValueObjects;

public class SimpleStringRequiredValueObject<T> : SimpleStringValueObject<T> where T : SimpleStringValueObject<T> {
    public SimpleStringRequiredValueObject(string value) : base(value) {

        if (string.IsNullOrWhiteSpace(value))
            throw new Exceptions.DomainException("The value cannot be null or empty");

    }
}