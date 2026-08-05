using System.ComponentModel.DataAnnotations;

namespace EcoFoodApi.Api.Models;

// No se modela como FK con cascada: el registro debe sobrevivir a la eliminación del Alimento de origen.
public class HistorialConsumo
{
    public int Id { get; set; }

    [Required]
    public int AlimentoId { get; set; }

    [Required]
    [MaxLength(100)]
    public string NombreAlimento { get; set; } = string.Empty;

    [Required]
    public DateTime FechaConsumo { get; set; }

    [Required]
    public bool SeDesperdicio { get; set; }
}
