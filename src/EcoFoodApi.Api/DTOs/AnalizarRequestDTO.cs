using System.ComponentModel.DataAnnotations;

namespace EcoFoodApi.Api.DTOs;

public class AnalizarRequestDTO
{
    [Required]
    [MinLength(1, ErrorMessage = "Debe indicar al menos un producto")]
    public List<string> Productos { get; set; } = new();
}
