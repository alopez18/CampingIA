namespace CampingAI.Infra.Models.CampingAI_DB;

public partial class T_CAMPINGS
{
    public Guid CMP_IdCamping { get; set; }

    public string CMP_Name { get; set; } = null!;

    public string CMP_Description { get; set; } = null!;

    public decimal CMP_Latitude { get; set; }

    public decimal CMP_Longitude { get; set; }

    public decimal CMP_PricePerNight { get; set; }

    public Guid CMP_OwnerId { get; set; }

    public int CMP_CategoryId { get; set; }

    public DateTime CMP_CreatedOn { get; set; }

    public DateTime CMP_UpdatedOn { get; set; }

    public DateTime? CMP_DeletedOn { get; set; }
}
