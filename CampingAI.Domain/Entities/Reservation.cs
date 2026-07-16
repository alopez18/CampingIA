namespace CampingAI.Domain.Entities;
public class Reservation : Abstractions.Entities.Deleteable, Abstractions.Entities.IAuditableEntity {

    public Guid UserId { get; private set; }
    public Guid CampingId { get; private set; }
    public ValueObjects.ReservationDateVO Dates { get; private set; }
    public ValueObjects.PriceVO TotalPrice { get; private set; }
    public int StatusId { get; private set; }

    public ValueObjects.DateFromPastVO CreatedOn { get; set; }
    public ValueObjects.DateFromPastVO UpdatedOn { get; set; }

    public Reservation(Guid idReservation,
                       Guid userId,
                       Guid campingId,
                       DateTime checkIn,
                       DateTime checkOut,
                       decimal totalPrice,
                       int statusId,
                       DateTime createdOn,
                       DateTime updatedOn,
                       DateTime? deletedOn) : base(idReservation, deletedOn) {
        if (idReservation == Guid.Empty)
            throw new ArgumentException("The reservation ID cannot be empty.", nameof(idReservation));

        UserId = userId;
        CampingId = campingId;
        Dates = new ValueObjects.ReservationDateVO(checkIn, checkOut);
        TotalPrice = new ValueObjects.PriceVO(totalPrice);
        StatusId = statusId;
        CreatedOn = new(createdOn);
        UpdatedOn = new(updatedOn);
    }

    public static Reservation CreateNew(Guid userId,
                                        Guid campingId,
                                        DateTime checkIn,
                                        DateTime checkOut,
                                        decimal totalPrice,
                                        int statusId) {
        return new Reservation(Guid.NewGuid(),
                               userId,
                               campingId,
                               checkIn,
                               checkOut,
                               totalPrice,
                               statusId,
                               DateTime.UtcNow,
                               DateTime.UtcNow,
                               null);
    }

    public void UpdateDates(DateTime checkIn, DateTime checkOut) {
        Dates = new ValueObjects.ReservationDateVO(checkIn, checkOut);
    }

    public void UpdateStatus(int statusId) {
        StatusId = statusId;
    }

    public void Cancel() {
        if (StatusId == (int)Enums.ReservationStatus.Cancelled)
            throw new Exceptions.DomainException("La reserva ya está cancelada.");
        StatusId = (int)Enums.ReservationStatus.Cancelled;
        Updated();
    }

    public void Updated() {
        UpdatedOn = ValueObjects.DateFromPastVO.CreateNow();
    }

    public void Created() {
        CreatedOn = ValueObjects.DateFromPastVO.CreateNow();
        UpdatedOn = ValueObjects.DateFromPastVO.CreateNow();
    }
}
