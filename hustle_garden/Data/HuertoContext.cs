using Microsoft.EntityFrameworkCore;
using HuertoApp.Models;

namespace HuertoApp.Data;

/// <summary>
/// Contexto de base de datos para la aplicación del huerto.
/// Maneja las entidades relacionadas con plantas, riegos, tareas, cosechas y notas.
/// </summary>
public class HuertoContext : DbContext
{
    /// <summary>
    /// Conjunto de datos de plantas.
    /// </summary>
    public DbSet<Planta> Plantas { get; set; }
    /// <summary>
    /// Conjunto de datos de riegos.
    /// </summary>
    public DbSet<Riego> Riegos { get; set; }
    /// <summary>
    /// Conjunto de datos de tareas.
    /// </summary>
    public DbSet<Tarea> Tareas { get; set; }
    /// <summary>
    /// Conjunto de datos de cosechas.
    /// </summary>
    public DbSet<Cosecha> Cosechas { get; set; }
    /// <summary>
    /// Conjunto de datos de notas del huerto.
    /// </summary>
    public DbSet<NotaHuerto> NotasHuerto { get; set; }

    /// <summary>
    /// Inicializa una nueva instancia de HuertoContext.
    /// Configura SQLite y asegura que la base de datos esté creada.
    /// </summary>
    public HuertoContext()
    {
        SQLitePCL.Batteries_V2.Init();
        
        this.Database.EnsureCreated();
        
       
       
    }

    /// <summary>
    /// Configura las opciones del contexto, específicamente la cadena de conexión a SQLite.
    /// </summary>
    /// <param name="optionsBuilder">Constructor de opciones para configurar el contexto.</param>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "huerto_ef.db3");
        optionsBuilder.UseSqlite($"Filename={dbPath}");
    }

    /// <summary>
    /// Configura el modelo de datos y las relaciones entre entidades.
    /// </summary>
    /// <param name="modelBuilder">Constructor del modelo para configurar entidades y relaciones.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Riego>()
            .HasOne(r => r.Planta)
            .WithMany(p => p.Riegos)
            .HasForeignKey(r => r.PlantaId);
            
        modelBuilder.Entity<Tarea>()
            .HasOne(t => t.Planta)
            .WithMany(p => p.Tareas)
            .HasForeignKey(t => t.PlantaId)
            .OnDelete(DeleteBehavior.SetNull);
            
        modelBuilder.Entity<Cosecha>()
            .HasOne(c => c.Planta)
            .WithMany(p => p.Cosechas)
            .HasForeignKey(c => c.PlantaId);
    }
}