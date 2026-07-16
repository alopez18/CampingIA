namespace CampingAI.DataImporter.Services.Interfaces;

public interface IProvinceGeoResolver {
    // Devuelve el PRV_Code de la provincia que contiene el punto indicado,
    // o null si ninguna geometría lo contiene.
    string? ResolveProvinceCode(double latitude, double longitude);
}
