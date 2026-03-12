using Microsoft.EntityFrameworkCore;
using RegistroInstrumentos.Data;
using RegistroInstrumentos.Models;

namespace RegistroInstrumentos.Repositories;

public class InstrumentoRepository
{
    private readonly AppDbContext _context;

    public InstrumentoRepository(AppDbContext context) => _context = context;

    public List<Instrumento> ObtenerTodos() =>
        _context.Instrumentos.Where(i => i.Activo).OrderBy(i => i.Nombre).ToList();

    public List<Instrumento> ObtenerDisponibles() =>
        _context.Instrumentos.Where(i => i.Activo && i.Estado == "Disponible").OrderBy(i => i.Nombre).ToList();

    public List<Instrumento> Buscar(string texto) =>
        _context.Instrumentos
            .Where(i => i.Activo && (i.Nombre.Contains(texto) || i.Codigo.Contains(texto) || i.Marca.Contains(texto)))
            .OrderBy(i => i.Nombre).ToList();

    public Instrumento? ObtenerPorId(int id) =>
        _context.Instrumentos.Include(i => i.Repuestos).FirstOrDefault(i => i.Id == id);

    public void Agregar(Instrumento instrumento)
    {
        _context.Instrumentos.Add(instrumento);
        _context.SaveChanges();
    }

    public void Actualizar(Instrumento instrumento)
    {
        _context.Instrumentos.Update(instrumento);
        _context.SaveChanges();
    }

    public void Eliminar(int id)
    {
        var instrumento = _context.Instrumentos.Find(id);
        if (instrumento != null)
        {
            instrumento.Activo = false;
            _context.SaveChanges();
        }
    }
}
