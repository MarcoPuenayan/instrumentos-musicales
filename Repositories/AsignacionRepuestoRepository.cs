using Microsoft.EntityFrameworkCore;
using RegistroInstrumentos.Data;
using RegistroInstrumentos.Models;

namespace RegistroInstrumentos.Repositories;

public class AsignacionRepuestoRepository
{
    private readonly AppDbContext _context;

    public AsignacionRepuestoRepository(AppDbContext context) => _context = context;

    public List<AsignacionRepuesto> ObtenerTodos() =>
        _context.AsignacionesRepuesto
            .Include(a => a.Repuesto)
            .Include(a => a.Persona)
            .OrderByDescending(a => a.FechaEntrega).ToList();

    public AsignacionRepuesto? ObtenerPorId(int id) =>
        _context.AsignacionesRepuesto
            .Include(a => a.Repuesto)
            .Include(a => a.Persona)
            .FirstOrDefault(a => a.Id == id);

    public void Agregar(AsignacionRepuesto asignacion)
    {
        _context.AsignacionesRepuesto.Add(asignacion);
        _context.SaveChanges();
    }
}
