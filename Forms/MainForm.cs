using RegistroInstrumentos.Data;
using RegistroInstrumentos.Repositories;
using RegistroInstrumentos.Services;

namespace RegistroInstrumentos.Forms;

public class MainForm : Form
{
    private readonly AppDbContext _context;
    private readonly AuthService _authService;
    private MenuStrip menuStrip = null!;
    private StatusStrip statusStrip = null!;
    private ToolStripStatusLabel lblEstado = null!;
    private ToolStripStatusLabel lblUsuario = null!;

    public MainForm(AppDbContext context, AuthService authService)
    {
        _context = context;
        _authService = authService;
        InicializarComponentes();
    }

    private void InicializarComponentes()
    {
        Text = "Sistema de Registro de Instrumentos Musicales";
        Size = new Size(1200, 750);
        StartPosition = FormStartPosition.CenterScreen;
        IsMdiContainer = true;
        WindowState = FormWindowState.Maximized;
        BackColor = Color.FromArgb(220, 230, 240);

        CrearMenuStrip();
        CrearStatusStrip();
        AplicarVisibilidadPorRol();
    }

    private void CrearMenuStrip()
    {
        menuStrip = new MenuStrip
        {
            BackColor = Color.FromArgb(30, 80, 120),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 10)
        };

        // Personas
        var mnuPersonas = new ToolStripMenuItem("Personas") { ForeColor = Color.White };
        mnuPersonas.DropDownItems.Add(CrearItem("Registrar / Gestionar Personas", MnuPersonas_Click));
        menuStrip.Items.Add(mnuPersonas);

        // Instrumentos
        var mnuInstrumentos = new ToolStripMenuItem("Instrumentos") { ForeColor = Color.White };
        mnuInstrumentos.DropDownItems.Add(CrearItem("Inventario de Instrumentos", MnuInstrumentos_Click));
        mnuInstrumentos.DropDownItems.Add(CrearItem("Asignar Instrumento", MnuAsignarInstrumento_Click));
        menuStrip.Items.Add(mnuInstrumentos);

        // Repuestos
        var mnuRepuestos = new ToolStripMenuItem("Repuestos") { ForeColor = Color.White };
        mnuRepuestos.DropDownItems.Add(CrearItem("Registro de Repuestos", MnuRepuestos_Click));
        mnuRepuestos.DropDownItems.Add(CrearItem("Asignar Repuesto a Persona", MnuAsignarRepuesto_Click));
        menuStrip.Items.Add(mnuRepuestos);

        // Administracion (solo admin)
        var mnuAdmin = new ToolStripMenuItem("Administracion") { ForeColor = Color.White, Tag = "Admin" };
        mnuAdmin.DropDownItems.Add(CrearItem("Gestion de Usuarios", MnuUsuarios_Click));
        menuStrip.Items.Add(mnuAdmin);

        // Cerrar sesion
        var mnuSesion = new ToolStripMenuItem("Cerrar Sesion") { ForeColor = Color.FromArgb(255, 180, 100) };
        mnuSesion.Click += MnuCerrarSesion_Click;
        menuStrip.Items.Add(mnuSesion);

        MainMenuStrip = menuStrip;
        Controls.Add(menuStrip);
    }

    private ToolStripMenuItem CrearItem(string texto, EventHandler handler)
    {
        var item = new ToolStripMenuItem(texto);
        item.Click += handler;
        return item;
    }

    private void CrearStatusStrip()
    {
        statusStrip = new StatusStrip { BackColor = Color.FromArgb(30, 80, 120) };

        lblUsuario = new ToolStripStatusLabel
        {
            ForeColor = Color.White,
            Text = $"Usuario: {AuthService.UsuarioActual?.NombreCompleto} ({AuthService.UsuarioActual?.Rol})"
        };

        lblEstado = new ToolStripStatusLabel
        {
            ForeColor = Color.FromArgb(180, 220, 255),
            Spring = true,
            TextAlign = ContentAlignment.MiddleRight,
            Text = $"Sistema de Instrumentos Musicales | {DateTime.Now:dd/MM/yyyy}"
        };

        statusStrip.Items.Add(lblUsuario);
        statusStrip.Items.Add(lblEstado);
        Controls.Add(statusStrip);
    }

    private void AplicarVisibilidadPorRol()
    {
        foreach (ToolStripMenuItem item in menuStrip.Items)
        {
            if (item.Tag?.ToString() == "Admin")
                item.Visible = _authService.EsAdministrador();
        }
    }

    private void AbrirFormuario<T>(Func<T> factory) where T : Form
    {
        foreach (Form f in MdiChildren)
        {
            if (f is T) { f.Activate(); return; }
        }
        var frm = factory();
        frm.MdiParent = this;
        frm.WindowState = FormWindowState.Maximized;
        frm.Show();
    }

    private void MnuPersonas_Click(object? s, EventArgs e) =>
        AbrirFormuario(() => new PersonaForm(new PersonaRepository(_context)));

    private void MnuInstrumentos_Click(object? s, EventArgs e) =>
        AbrirFormuario(() => new InstrumentoForm(new InstrumentoRepository(_context)));

    private void MnuAsignarInstrumento_Click(object? s, EventArgs e) =>
        AbrirFormuario(() => new AsignacionInstrumentoForm(
            new AsignacionInstrumentoRepository(_context),
            new InstrumentoRepository(_context),
            new PersonaRepository(_context)));

    private void MnuRepuestos_Click(object? s, EventArgs e) =>
        AbrirFormuario(() => new RepuestoForm(
            new RepuestoRepository(_context),
            new InstrumentoRepository(_context)));

    private void MnuAsignarRepuesto_Click(object? s, EventArgs e) =>
        AbrirFormuario(() => new AsignacionRepuestoForm(
            new AsignacionRepuestoRepository(_context),
            new RepuestoRepository(_context),
            new PersonaRepository(_context)));

    private void MnuUsuarios_Click(object? s, EventArgs e) =>
        AbrirFormuario(() => new UsuarioForm(new UsuarioRepository(_context)));

    private void MnuCerrarSesion_Click(object? s, EventArgs e)
    {
        if (MessageBox.Show("¿Desea cerrar la sesion actual?", "Confirmar",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        {
            _authService.Logout();
            Application.Restart();
        }
    }
}
