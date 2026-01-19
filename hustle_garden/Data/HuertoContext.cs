using Microsoft.EntityFrameworkCore;
using HuertoApp.Models;

namespace HuertoApp.Data;

public class HuertoContext : DbContext
{
    public DbSet<Planta> Plantas { get; set; }

    public HuertoContext()
    {
        // Esto asegura que la base de datos se cree en el móvil al iniciar
        SQLitePCL.Batteries_V2.Init();
        this.Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Ruta local en el dispositivo
        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "huerto_ef.db3");
        optionsBuilder.UseSqlite($"Filename={dbPath}");
    }
}