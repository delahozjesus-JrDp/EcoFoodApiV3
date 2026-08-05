namespace EcoFoodApi.Api.DTOs;

public class AlimentoDTO
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public decimal Cantidad { get; set; }
    public string Unidad { get; set; } = string.Empty;
    public DateTime FechaIngreso { get; set; }
    public DateTime FechaVencimiento { get; set; }
    public bool Consumido { get; set; }
}
