namespace CampingAI.DataImporter.Models;

public class T_CAMPINGS_IMPORT {
    public Guid     CMI_IdCamping  { get; set; }
    public string   CMI_ExternalId { get; set; } = null!;
    public string   CMI_Source     { get; set; } = null!;
    public string   CMI_Name       { get; set; } = null!;
    public decimal? CMI_Latitude   { get; set; }
    public decimal? CMI_Longitude  { get; set; }
    public string?  CMI_Address    { get; set; }
    public string?  CMI_PostalCode { get; set; }
    public string?  CMI_City       { get; set; }
    public string?  CMI_Province   { get; set; }
    public string?  CMI_Country    { get; set; }
    public string?  CMI_Phone      { get; set; }
    public string?  CMI_Email      { get; set; }
    public string?  CMI_Website    { get; set; }
    public DateTime CMI_CreatedOn  { get; set; }
    public DateTime CMI_UpdatedOn  { get; set; }
}
