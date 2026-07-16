namespace CampingAI.Domain.Entities;
public class CampingCategory : Abstractions.Entities.Entity {

    public Guid CampingId { get; private set; }
    public Guid CategoryId { get; private set; }

    public CampingCategory(Guid idCampingCategory,
                           Guid campingId,
                           Guid categoryId) : base(idCampingCategory) {
        if (idCampingCategory == Guid.Empty)
            throw new ArgumentException("The CampingCategory ID cannot be empty.", nameof(idCampingCategory));
        if (campingId == Guid.Empty)
            throw new Exceptions.DomainException("CampingId cannot be empty.");
        if (categoryId == Guid.Empty)
            throw new Exceptions.DomainException("CategoryId cannot be empty.");

        CampingId = campingId;
        CategoryId = categoryId;
    }

    public static CampingCategory CreateNew(Guid campingId, Guid categoryId) {
        return new CampingCategory(Guid.NewGuid(), campingId, categoryId);
    }
}
