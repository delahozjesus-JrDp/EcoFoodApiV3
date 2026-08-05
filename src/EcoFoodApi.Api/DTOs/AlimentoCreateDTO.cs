using System.ComponentModel.DataAnnotations;

namespace EcoFoodApi.Api.DTOs;

public class AlimentoCreateDTO
{
    [Required]
    [MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Categoria { get; set; } = string.Empty;

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
    public decimal Cantidad { get; set; }

    [Required]
    [MaxLength(20)]
    public string Unidad { get; set; } = string.Empty;

    [Required]
    public DateTime FechaIngreso { get; set; }

    [Required]
    public DateTime FechaVencimiento { get; set; }
}
