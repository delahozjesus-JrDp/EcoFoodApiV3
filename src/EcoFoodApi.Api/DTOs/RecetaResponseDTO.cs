using System.Text.Json.Serialization;

namespace EcoFoodApi.Api.DTOs;

public class RecetaResponseDTO
{
    [JsonPropertyName("receta")]
    public string? Receta { get; set; }

    [JsonPropertyName("ingredientesFaltantes")]
    public List<string> IngredientesFaltantes { get; set; } = new();

    [JsonPropertyName("productosConsumirPrimero")]
    public List<string> ProductosConsumirPrimero { get; set; } = new();

    [JsonPropertyName("mensaje")]
    public string? Mensaje { get; set; }
}
