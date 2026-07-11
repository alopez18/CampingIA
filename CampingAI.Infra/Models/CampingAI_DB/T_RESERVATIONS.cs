namespace CampingAI.Infra.Models.CampingAI_DB;

public partial class T_RESERVATIONS
{
    public Guid RES_IdReservation { get; set; }

    public Guid RES_UserId { get; set; }

    public Guid RES_CampingId { get; set; }

    public DateTime RES_CheckIn { get; set; }

    public DateTime RES_CheckOut { get; set; }

    public decimal RES_TotalPrice { get; set; }

    public int RES_StatusId { get; set; }

    public DateTime RES_CreatedOn { get; set; }

    public DateTime RES_UpdatedOn { get; set; }

    public DateTime? RES_DeletedOn { get; set; }
}
