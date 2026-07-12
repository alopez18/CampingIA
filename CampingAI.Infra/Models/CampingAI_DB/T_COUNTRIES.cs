namespace CampingAI.Infra.Models.CampingAI_DB;

public partial class T_COUNTRIES
{
    public Guid CNT_IdCountry { get; set; }

    public string CNT_Code { get; set; } = null!;

    public string CNT_Name { get; set; } = null!;
}
