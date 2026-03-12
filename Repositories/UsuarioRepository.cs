using RegistroInstrumentos.Data;
using RegistroInstrumentos.Models;

namespace RegistroInstrumentos.Repositories;

public class UsuarioRepository
{
    private readonly AppDbContext _context;

    public UsuarioRepository(AppDbContext context) => _context = context;

    public List<Usuario> ObtenerTodos() =>
        _context.Usuarios.OrderBy(u => u.NombreUsuario).ToList();

    public Usuario? ObtenerPorId(int id) =>
        _context.Usuarios.Find(id);

    public Usuario? ObtenerPorNombreUsuario(string nombreUsuario) =>
        _context.Usuarios.FirstOrDefault(u => u.NombreUsuario == nombreUsuario);

    public void Agregar(Usuario usuario)
    {
        _context.Usuarios.Add(usuario);
        _context.SaveChanges();
    }

    public void Actualizar(Usuario usuario)
    {
        _context.Usuarios.Update(usuario);
        _context.SaveChanges();
    }

    public void Eliminar(int id)
    {
        var usuario = _context.Usuarios.Find(id);
        if (usuario != null)
        {
            usuario.Activo = false;
            _context.SaveChanges();
        }
    }
}
