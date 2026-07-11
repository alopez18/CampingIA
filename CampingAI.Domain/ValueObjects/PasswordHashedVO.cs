namespace CampingAI.Domain.ValueObjects;
public class PasswordHashedVO : Abstractions.ValueObjects.SimpleStringRequiredValueObject<PasswordHashedVO> {
    public PasswordHashedVO(string value) : base(value) {


    }
}