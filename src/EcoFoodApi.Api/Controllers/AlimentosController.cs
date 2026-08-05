using EcoFoodApi.Api.Data;
using EcoFoodApi.Api.DTOs;
using EcoFoodApi.Api.Models;
using EcoFoodApi.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcoFoodApi.Api.Controllers;

[ApiController]
[Route("api/alimentos")]
public class AlimentosController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly GroqService _groqService;

    public AlimentosController(AppDbContext context, GroqService groqService)
    {
        _context = context;
        _groqService = groqService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AlimentoDTO>>> GetAlimentos()
    {
        var alimentos = await _context.Alimentos
            .Select(a => MapToDTO(a))
            .ToListAsync();

        return Ok(alimentos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AlimentoDTO>> GetAlimento(int id)
    {
        var alimento = await _context.Alimentos.FindAsync(id);

        if (alimento == null)
        {
            return NotFound(new { mensaje = $"No se encontró el alimento con Id {id}" });
        }

        return Ok(MapToDTO(alimento));
    }

    [HttpPost]
    public async Task<ActionResult<AlimentoDTO>> CrearAlimento(AlimentoCreateDTO dto)
    {
        var alimento = new Alimento
        {
            Nombre = dto.Nombre,
            Categoria = dto.Categoria,
            Cantidad = dto.Cantidad,
            Unidad = dto.Unidad,
            FechaIngreso = dto.FechaIngreso,
            FechaVencimiento = dto.FechaVencimiento,
            Consumido = false
        };

        _context.Alimentos.Add(alimento);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAlimento), new { id = alimento.Id }, MapToDTO(alimento));
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<AlimentoDTO>> ActualizarAlimento(int id, AlimentoUpdateDTO dto)
    {
        var alimento = await _context.Alimentos.FindAsync(id);

        if (alimento == null)
        {
            return NotFound(new { mensaje = $"No se encontró el alimento con Id {id}" });
        }

        alimento.Nombre = dto.Nombre;
        alimento.Categoria = dto.Categoria;
        alimento.Cantidad = dto.Cantidad;
        alimento.Unidad = dto.Unidad;
        alimento.FechaVencimiento = dto.FechaVencimiento;
        alimento.Consumido = dto.Consumido;

        await _context.SaveChangesAsync();

        return Ok(MapToDTO(alimento));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> EliminarAlimento(int id)
    {
        var alimento = await _context.Alimentos.FindAsync(id);

        if (alimento == null)
        {
            return NotFound(new { mensaje = $"No se encontró el alimento con Id {id}" });
        }

        _context.HistorialesConsumo.Add(new HistorialConsumo
        {
            AlimentoId = alimento.Id,
            NombreAlimento = alimento.Nombre,
            FechaConsumo = DateTime.UtcNow,
            SeDesperdicio = alimento.FechaVencimiento < DateTime.UtcNow
        });

        _context.Alimentos.Remove(alimento);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("proximos-a-vencer")]
    public async Task<ActionResult<IEnumerable<AlimentoDTO>>> GetProximosAVencer([FromQuery] int dias = 3)
    {
        var limite = DateTime.UtcNow.Date.AddDays(dias);

        var alimentos = await _context.Alimentos
            .Where(a => !a.Consumido && a.FechaVencimiento <= limite)
            .OrderBy(a => a.FechaVencimiento)
            .Select(a => MapToDTO(a))
            .ToListAsync();

        return Ok(alimentos);
    }

    [HttpGet("buscar")]
    public async Task<ActionResult<IEnumerable<AlimentoDTO>>> Buscar([FromQuery] string? nombre, [FromQuery] string? categoria)
    {
        var query = _context.Alimentos.AsQueryable();

        if (!string.IsNullOrWhiteSpace(nombre))
        {
            query = query.Where(a => a.Nombre.Contains(nombre));
        }

        if (!string.IsNullOrWhiteSpace(categoria))
        {
            query = query.Where(a => a.Categoria.Contains(categoria));
        }

        var alimentos = await query.Select(a => MapToDTO(a)).ToListAsync();

        return Ok(alimentos);
    }

    [HttpGet("estadisticas")]
    public async Task<ActionResult<EstadisticasDTO>> GetEstadisticas()
    {
        var hoy = DateTime.UtcNow.Date;
        var alimentos = await _context.Alimentos.ToListAsync();

        var estadisticas = new EstadisticasDTO
        {
            TotalAlimentos = alimentos.Count,
            ProximosAVencer = alimentos.Count(a => !a.Consumido && a.FechaVencimiento >= hoy && a.FechaVencimiento <= hoy.AddDays(3)),
            Vencidos = alimentos.Count(a => !a.Consumido && a.FechaVencimiento < hoy),
            Consumidos = alimentos.Count(a => a.Consumido),
            PorCategoria = alimentos
                .GroupBy(a => a.Categoria)
                .ToDictionary(g => g.Key, g => g.Count())
        };

        return Ok(estadisticas);
    }

    [HttpPost("analizar")]
    public async Task<ActionResult<RecetaResponseDTO>> Analizar(AnalizarRequestDTO dto)
    {
        var receta = await _groqService.ObtenerRecetaAsync(dto.Productos);
        return Ok(receta);
    }

    private static AlimentoDTO MapToDTO(Alimento a) => new()
    {
        Id = a.Id,
        Nombre = a.Nombre,
        Categoria = a.Categoria,
        Cantidad = a.Cantidad,
        Unidad = a.Unidad,
        FechaIngreso = a.FechaIngreso,
        FechaVencimiento = a.FechaVencimiento,
        Consumido = a.Consumido
    };
}
