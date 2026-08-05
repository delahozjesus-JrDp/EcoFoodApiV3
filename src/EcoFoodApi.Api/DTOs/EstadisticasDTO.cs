namespace EcoFoodApi.Api.DTOs;

public class EstadisticasDTO
{
    public int TotalAlimentos { get; set; }
    public int ProximosAVencer { get; set; }
    public int Vencidos { get; set; }
    public int Consumidos { get; set; }
    public Dictionary<string, int> PorCategoria { get; set; } = new();
}
