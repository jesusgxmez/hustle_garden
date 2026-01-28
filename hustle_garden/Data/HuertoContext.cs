using Microsoft.EntityFrameworkCore;
using HuertoApp.Models;

namespace HuertoApp.Data;

public class HuertoContext : DbContext
{
    public DbSet<Planta> Plantas { get; set; }
    public DbSet<Riego> Riegos { get; set; }
    public DbSet<Tarea> Tareas { get; set; }
    public DbSet<Cosecha> Cosechas { get; set; }
    public DbSet<NotaHuerto> NotasHuerto { get; set; }

    public HuertoContext()
    {
        SQLitePCL.Batteries_V2.Init();
        
        this.Database.EnsureCreated();
        
       
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "huerto_ef.db3");
        optionsBuilder.UseSqlite($"Filename={dbPath}");
    }

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