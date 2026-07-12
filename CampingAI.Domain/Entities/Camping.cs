namespace CampingAI.Domain.Entities;
public class Camping : Abstractions.Entities.Deleteable, Abstractions.Entities.IAuditableEntity {

    public ValueObjects.CampingNameVO Name { get; private set; }
    public ValueObjects.CampingDescriptionVO Description { get; private set; }
    public ValueObjects.LatitudeVO Latitude { get; private set; }
    public ValueObjects.LongitudeVO Longitude { get; private set; }
    public ValueObjects.PriceVO PricePerNight { get; private set; }
    public Guid OwnerId { get; private set; }
    public int CategoryId { get; private set; }
    public Guid? ProvinciaId { get; private set; }

    public ValueObjects.DateFromPastVO CreatedOn { get; set; }
    public ValueObjects.DateFromPastVO UpdatedOn { get; set; }

    private readonly List<Guid> _facilityIds = [];
    public IReadOnlyList<Guid> FacilityIds => _facilityIds.AsReadOnly();

    public Camping(Guid idCamping,
                   string name,
                   string description,
                   decimal latitude,
                   decimal longitude,
                   decimal pricePerNight,
                   Guid ownerId,
                   int categoryId,
                   Guid? provinciaId,
                   DateTime createdOn,
                   DateTime updatedOn,
                   DateTime? deletedOn) : base(idCamping, deletedOn) {
        if (idCamping == Guid.Empty)
            throw new ArgumentException("The camping ID cannot be empty.", nameof(idCamping));

        Name = new ValueObjects.CampingNameVO(name);
        Description = new ValueObjects.CampingDescriptionVO(description);
        Latitude = new ValueObjects.LatitudeVO(latitude);
        Longitude = new ValueObjects.LongitudeVO(longitude);
        PricePerNight = new ValueObjects.PriceVO(pricePerNight);
        OwnerId = ownerId;
        CategoryId = categoryId;
        ProvinciaId = provinciaId;
        CreatedOn = new(createdOn);
        UpdatedOn = new(updatedOn);
    }

    public static Camping CreateNew(string name,
                                    string description,
                                    decimal latitude,
                                    decimal longitude,
                                    decimal pricePerNight,
                                    Guid ownerId,
                                    int categoryId,
                                    Guid? provinciaId = null) {
        return new Camping(Guid.NewGuid(),
                           name,
                           description,
                           latitude,
                           longitude,
                           pricePerNight,
                           ownerId,
                           categoryId,
                           provinciaId,
                           DateTime.Now,
                           DateTime.Now,
                           null);
    }

    public void UpdateDetails(string name, string description, decimal pricePerNight, int categoryId,
                              Guid? provinciaId = null) {
        Name = new ValueObjects.CampingNameVO(name);
        Description = new ValueObjects.CampingDescriptionVO(description);
        PricePerNight = new ValueObjects.PriceVO(pricePerNight);
        CategoryId = categoryId;
        ProvinciaId = provinciaId;
    }

    public void UpdateLocation(decimal latitude, decimal longitude) {
        Latitude = new ValueObjects.LatitudeVO(latitude);
        Longitude = new ValueObjects.LongitudeVO(longitude);
    }

    public void SetFacilities(IEnumerable<Guid> facilityIds) {
        _facilityIds.Clear();
        _facilityIds.AddRange(facilityIds);
    }

    public void AddFacility(Guid facilityId) {
        if (facilityId == Guid.Empty)
            throw new Exceptions.DomainException("FacilityId cannot be empty.");
        if (!_facilityIds.Contains(facilityId))
            _facilityIds.Add(facilityId);
    }

    public void RemoveFacility(Guid facilityId) {
        _facilityIds.Remove(facilityId);
    }

    public void Updated() {
        UpdatedOn = ValueObjects.DateFromPastVO.CreateNow();
    }

    public void Created() {
        CreatedOn = ValueObjects.DateFromPastVO.CreateNow();
        UpdatedOn = ValueObjects.DateFromPastVO.CreateNow();
    }
}
