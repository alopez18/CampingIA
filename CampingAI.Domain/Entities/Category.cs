namespace CampingAI.Domain.Entities;
public class Category : Abstractions.Entities.Entity {

    public ValueObjects.CampingNameVO Name { get; private set; }

    public Category(Guid idCategory, string name) : base(idCategory) {
        if (idCategory == Guid.Empty)
            throw new ArgumentException("The category ID cannot be empty.", nameof(idCategory));

        Name = new ValueObjects.CampingNameVO(name);
    }

    public static Category CreateNew(string name) {
        return new Category(Guid.NewGuid(), name);
    }

    public void UpdateName(string name) {
        Name = new ValueObjects.CampingNameVO(name);
    }
}
