namespace CampingAI.Domain.Entities;
public class CampingFacility : Abstractions.Entities.Entity {

    public Guid CampingId { get; private set; }
    public Guid FacilityId { get; private set; }

    public CampingFacility(Guid idCampingFacility,
                           Guid campingId,
                           Guid facilityId) : base(idCampingFacility) {
        if (idCampingFacility == Guid.Empty)
            throw new ArgumentException("The CampingFacility ID cannot be empty.", nameof(idCampingFacility));
        if (campingId == Guid.Empty)
            throw new Exceptions.DomainException("CampingId cannot be empty.");
        if (facilityId == Guid.Empty)
            throw new Exceptions.DomainException("FacilityId cannot be empty.");

        CampingId = campingId;
        FacilityId = facilityId;
    }

    public static CampingFacility CreateNew(Guid campingId, Guid facilityId) {
        return new CampingFacility(Guid.NewGuid(), campingId, facilityId);
    }
}
