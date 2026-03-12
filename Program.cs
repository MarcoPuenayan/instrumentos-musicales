using RegistroInstrumentos.Data;
using RegistroInstrumentos.Forms;
using RegistroInstrumentos.Repositories;
using RegistroInstrumentos.Services;

namespace RegistroInstrumentos;

static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        using var context = new AppDbContext();
        context.InicializarBaseDatos();

        var usuarioRepo = new UsuarioRepository(context);
        var authService = new AuthService(usuarioRepo);

        using var loginForm = new LoginForm(authService);
        if (loginForm.ShowDialog() != DialogResult.OK)
            return;

        Application.Run(new MainForm(context, authService));
    }
}