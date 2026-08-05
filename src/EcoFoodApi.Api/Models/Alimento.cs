using System.ComponentModel.DataAnnotations;

namespace EcoFoodApi.Api.Models;

public class Alimento
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Categoria { get; set; } = string.Empty;

    [Required]
    public decimal Cantidad { get; set; }

    [Required]
    [MaxLength(20)]
    public string Unidad { get; set; } = string.Empty;

    [Required]
    public DateTime FechaIngreso { get; set; }

    [Required]
    public DateTime FechaVencimiento { get; set; }

    public bool Consumido { get; set; } = false;
}
