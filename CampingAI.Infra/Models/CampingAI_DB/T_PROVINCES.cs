namespace CampingAI.Infra.Models.CampingAI_DB;

public partial class T_PROVINCES
{
    public Guid PRV_IdProvince { get; set; }

    public string PRV_Code { get; set; } = null!;

    public string PRV_Name { get; set; } = null!;

    public Guid PRV_CountryId { get; set; }
}
