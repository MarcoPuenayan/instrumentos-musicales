using Microsoft.EntityFrameworkCore;
using RegistroInstrumentos.Data;
using RegistroInstrumentos.Models;

namespace RegistroInstrumentos.Repositories;

public class PersonaRepository
{
    private readonly AppDbContext _context;

    public PersonaRepository(AppDbContext context) => _context = context;

    public List<Persona> ObtenerTodos() =>
        _context.Personas.Where(p => p.Activo).OrderBy(p => p.Apellidos).ToList();

    public List<Persona> BuscarPorNombre(string texto) =>
        _context.Personas
            .Where(p => p.Activo && (p.Nombres.Contains(texto) || p.Apellidos.Contains(texto) || p.Cedula.Contains(texto)))
            .OrderBy(p => p.Apellidos).ToList();

    public Persona? ObtenerPorId(int id) =>
        _context.Personas.FirstOrDefault(p => p.Id == id);

    public Persona? ObtenerPorCedula(string cedula) =>
        _context.Personas.FirstOrDefault(p => p.Cedula == cedula);

    public void Agregar(Persona persona)
    {
        _context.Personas.Add(persona);
        _context.SaveChanges();
    }

    public void Actualizar(Persona persona)
    {
        _context.Personas.Update(persona);
        _context.SaveChanges();
    }

    public void Eliminar(int id)
    {
        var persona = _context.Personas.Find(id);
        if (persona != null)
        {
            persona.Activo = false;
            _context.SaveChanges();
        }
    }
}
