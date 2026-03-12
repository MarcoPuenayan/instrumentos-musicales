using RegistroInstrumentos.Helpers;
using RegistroInstrumentos.Models;
using RegistroInstrumentos.Repositories;

namespace RegistroInstrumentos.Services;

public class AuthService
{
    private readonly UsuarioRepository _usuarioRepo;

    public static Usuario? UsuarioActual { get; private set; }

    public AuthService(UsuarioRepository usuarioRepo) => _usuarioRepo = usuarioRepo;

    public bool Login(string nombreUsuario, string contrasena)
    {
        var usuario = _usuarioRepo.ObtenerPorNombreUsuario(nombreUsuario);
        if (usuario == null || !usuario.Activo) return false;
        if (!PasswordHelper.VerifyPassword(contrasena, usuario.Contrasena)) return false;

        UsuarioActual = usuario;
        return true;
    }

    public void Logout() => UsuarioActual = null;

    public bool EsAdministrador() => UsuarioActual?.Rol == "Administrador";
}
