using Microsoft.EntityFrameworkCore;
using RegistroInstrumentos.Data;
using RegistroInstrumentos.Models;

namespace RegistroInstrumentos.Repositories;

public class AsignacionInstrumentoRepository
{
    private readonly AppDbContext _context;

    public AsignacionInstrumentoRepository(AppDbContext context) => _context = context;

    public List<AsignacionInstrumento> ObtenerTodos() =>
        _context.AsignacionesInstrumento
            .Include(a => a.Instrumento)
            .Include(a => a.Persona)
            .OrderByDescending(a => a.FechaAsignacion).ToList();

    public List<AsignacionInstrumento> ObtenerActivos() =>
        _context.AsignacionesInstrumento
            .Include(a => a.Instrumento)
            .Include(a => a.Persona)
            .Where(a => a.Estado == "Activa")
            .OrderByDescending(a => a.FechaAsignacion).ToList();

    public AsignacionInstrumento? ObtenerPorId(int id) =>
        _context.AsignacionesInstrumento
            .Include(a => a.Instrumento)
            .Include(a => a.Persona)
            .FirstOrDefault(a => a.Id == id);

    public void Agregar(AsignacionInstrumento asignacion)
    {
        _context.AsignacionesInstrumento.Add(asignacion);
        _context.SaveChanges();
    }

    public void Actualizar(AsignacionInstrumento asignacion)
    {
        _context.AsignacionesInstrumento.Update(asignacion);
        _context.SaveChanges();
    }
}
