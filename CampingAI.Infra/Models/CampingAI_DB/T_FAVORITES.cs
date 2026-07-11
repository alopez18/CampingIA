namespace CampingAI.Infra.Models.CampingAI_DB;

public partial class T_FAVORITES
{
    public Guid FAV_IdFavorite { get; set; }

    public Guid FAV_UserId { get; set; }

    public Guid FAV_CampingId { get; set; }

    public DateTime FAV_CreatedAt { get; set; }
}
