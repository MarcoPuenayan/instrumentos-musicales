using Microsoft.EntityFrameworkCore;
using RegistroInstrumentos.Data;
using RegistroInstrumentos.Models;

namespace RegistroInstrumentos.Repositories;

public class RepuestoRepository
{
    private readonly AppDbContext _context;

    public RepuestoRepository(AppDbContext context) => _context = context;

    public List<Repuesto> ObtenerTodos() =>
        _context.Repuestos
            .Include(r => r.Instrumento)
            .Where(r => r.Activo).OrderBy(r => r.Nombre).ToList();

    public List<Repuesto> Buscar(string texto) =>
        _context.Repuestos
            .Include(r => r.Instrumento)
            .Where(r => r.Activo && (r.Nombre.Contains(texto) || r.Codigo.Contains(texto)))
            .OrderBy(r => r.Nombre).ToList();

    public Repuesto? ObtenerPorId(int id) =>
        _context.Repuestos.Include(r => r.Instrumento).FirstOrDefault(r => r.Id == id);

    public void Agregar(Repuesto repuesto)
    {
        _context.Repuestos.Add(repuesto);
        _context.SaveChanges();
    }

    public void Actualizar(Repuesto repuesto)
    {
        _context.Repuestos.Update(repuesto);
        _context.SaveChanges();
    }

    public void Eliminar(int id)
    {
        var repuesto = _context.Repuestos.Find(id);
        if (repuesto != null)
        {
            repuesto.Activo = false;
            _context.SaveChanges();
        }
    }
}
