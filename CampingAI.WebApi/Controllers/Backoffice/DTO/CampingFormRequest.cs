using System.ComponentModel.DataAnnotations;

namespace CampingAI.WebApi.Controllers.Backoffice.DTO;
public class CampingFormRequest {
    public Guid? Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "La descripción es obligatoria.")]
    public string Description { get; set; } = string.Empty;

    [Range(-90, 90, ErrorMessage = "La latitud debe estar entre -90 y 90.")]
    public decimal Latitude { get; set; }

    [Range(-180, 180, ErrorMessage = "La longitud debe estar entre -180 y 180.")]
    public decimal Longitude { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "El precio no puede ser negativo.")]
    public decimal PricePerNight { get; set; }

    public Guid CategoryId { get; set; }

    public Guid? ProvinciaId { get; set; }
}
