using EcoFoodApi.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace EcoFoodApi.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Alimento> Alimentos => Set<Alimento>();
    public DbSet<HistorialConsumo> HistorialesConsumo => Set<HistorialConsumo>();
}
