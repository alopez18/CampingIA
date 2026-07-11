namespace CampingAI.Domain.ValueObjects;
public class CampingNameVO : Abstractions.ValueObjects.SimpleStringRequiredValueObject<CampingNameVO> {
    public CampingNameVO(string value) : base(value) { }
}
