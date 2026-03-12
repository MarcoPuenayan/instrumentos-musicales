using Microsoft.EntityFrameworkCore;
using RegistroInstrumentos.Models;

namespace RegistroInstrumentos.Data;

public class AppDbContext : DbContext
{
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Persona> Personas { get; set; }
    public DbSet<Instrumento> Instrumentos { get; set; }
    public DbSet<AsignacionInstrumento> AsignacionesInstrumento { get; set; }
    public DbSet<Repuesto> Repuestos { get; set; }
    public DbSet<AsignacionRepuesto> AsignacionesRepuesto { get; set; }

    private static string DbPath =>
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "registro_instrumentos.db");

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite($"Data Source={DbPath}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Instrumento>()
            .Property(i => i.ValorAdquisicion)
            .HasColumnType("TEXT");

        modelBuilder.Entity<Repuesto>()
            .Property(r => r.Costo)
            .HasColumnType("TEXT");

        modelBuilder.Entity<AsignacionInstrumento>()
            .HasOne(a => a.Instrumento)
            .WithMany(i => i.Asignaciones)
            .HasForeignKey(a => a.InstrumentoId);

        modelBuilder.Entity<AsignacionInstrumento>()
            .HasOne(a => a.Persona)
            .WithMany(p => p.AsignacionesInstrumento)
            .HasForeignKey(a => a.PersonaId);

        modelBuilder.Entity<AsignacionRepuesto>()
            .HasOne(a => a.Repuesto)
            .WithMany(r => r.Asignaciones)
            .HasForeignKey(a => a.RepuestoId);

        modelBuilder.Entity<AsignacionRepuesto>()
            .HasOne(a => a.Persona)
            .WithMany(p => p.AsignacionesRepuesto)
            .HasForeignKey(a => a.PersonaId);

        modelBuilder.Entity<Repuesto>()
            .HasOne(r => r.Instrumento)
            .WithMany(i => i.Repuestos)
            .HasForeignKey(r => r.InstrumentoId)
            .IsRequired(false);
    }

    public void InicializarBaseDatos()
    {
        Database.EnsureCreated();
        SeedAdminUsuario();
    }

    private void SeedAdminUsuario()
    {
        if (!Usuarios.Any())
        {
            Usuarios.Add(new Usuario
            {
                NombreUsuario = "admin",
                Contrasena = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                NombreCompleto = "Administrador del Sistema",
                Rol = "Administrador",
                Activo = true,
                FechaCreacion = DateTime.Now
            });
            SaveChanges();
        }
    }
}
