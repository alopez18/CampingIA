namespace CampingAI.Domain.ValueObjects;
public class CampingDescriptionVO : Abstractions.ValueObjects.SimpleStringRequiredValueObject<CampingDescriptionVO> {
    public CampingDescriptionVO(string value) : base(value) { }
}
