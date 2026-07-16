namespace CampingAI.Domain.ValueObjects;
public class ReservationDateVO {
    public DateTime CheckIn { get; private set; }
    public DateTime CheckOut { get; private set; }

    public ReservationDateVO(DateTime checkIn, DateTime checkOut) {
        if (checkIn >= checkOut)
            throw new Exceptions.DomainException("Check-in date must be before check-out date.");
        if (checkIn < DateTime.UtcNow.Date)
            throw new Exceptions.DomainException("Check-in date must not be in the past.");
        CheckIn = checkIn;
        CheckOut = checkOut;
    }

    public int Nights => (CheckOut - CheckIn).Days;

    public override string ToString() => $"{CheckIn:yyyy-MM-dd} → {CheckOut:yyyy-MM-dd}";
    public override bool Equals(object obj) => obj is ReservationDateVO other && CheckIn == other.CheckIn && CheckOut == other.CheckOut;
    public override int GetHashCode() => HashCode.Combine(CheckIn, CheckOut);
}
