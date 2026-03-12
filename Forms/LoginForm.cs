using RegistroInstrumentos.Data;
using RegistroInstrumentos.Repositories;
using RegistroInstrumentos.Services;

namespace RegistroInstrumentos.Forms;

public class LoginForm : Form
{
    private readonly AuthService _authService;
    private TextBox txtUsuario = null!;
    private TextBox txtContrasena = null!;
    private Button btnIngresar = null!;
    private Label lblError = null!;

    public LoginForm(AuthService authService)
    {
        _authService = authService;
        InicializarComponentes();
    }

    private void InicializarComponentes()
    {
        Text = "Sistema de Registro de Instrumentos Musicales";
        Size = new Size(440, 380);
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        BackColor = Color.FromArgb(240, 244, 248);

        // Panel principal con sombra visual
        var pnlCard = new Panel
        {
            Size = new Size(360, 280),
            Location = new Point(40, 40),
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle
        };
        Controls.Add(pnlCard);

        // Titulo
        var lblTitulo = new Label
        {
            Text = "Iniciar Sesion",
            Font = new Font("Segoe UI", 18, FontStyle.Bold),
            ForeColor = Color.FromArgb(30, 80, 120),
            Location = new Point(0, 20),
            Size = new Size(360, 45),
            TextAlign = ContentAlignment.MiddleCenter
        };
        pnlCard.Controls.Add(lblTitulo);

        var lblSubtitulo = new Label
        {
            Text = "Instrumentos Musicales",
            Font = new Font("Segoe UI", 10, FontStyle.Regular),
            ForeColor = Color.FromArgb(100, 120, 140),
            Location = new Point(0, 60),
            Size = new Size(360, 25),
            TextAlign = ContentAlignment.MiddleCenter
        };
        pnlCard.Controls.Add(lblSubtitulo);

        // Usuario
        var lblUsuario = new Label
        {
            Text = "Usuario:",
            Font = new Font("Segoe UI", 10),
            Location = new Point(30, 105),
            Size = new Size(80, 22)
        };
        pnlCard.Controls.Add(lblUsuario);

        txtUsuario = new TextBox
        {
            Location = new Point(30, 128),
            Size = new Size(300, 30),
            Font = new Font("Segoe UI", 11),
            BorderStyle = BorderStyle.FixedSingle
        };
        pnlCard.Controls.Add(txtUsuario);

        // Contrasena
        var lblContrasena = new Label
        {
            Text = "Contrasena:",
            Font = new Font("Segoe UI", 10),
            Location = new Point(30, 165),
            Size = new Size(100, 22)
        };
        pnlCard.Controls.Add(lblContrasena);

        txtContrasena = new TextBox
        {
            Location = new Point(30, 188),
            Size = new Size(300, 30),
            Font = new Font("Segoe UI", 11),
            BorderStyle = BorderStyle.FixedSingle,
            PasswordChar = '*'
        };
        pnlCard.Controls.Add(txtContrasena);

        // Error
        lblError = new Label
        {
            Text = string.Empty,
            Font = new Font("Segoe UI", 9),
            ForeColor = Color.Red,
            Location = new Point(30, 220),
            Size = new Size(300, 20),
            TextAlign = ContentAlignment.MiddleCenter
        };
        pnlCard.Controls.Add(lblError);

        // Boton Ingresar
        btnIngresar = new Button
        {
            Text = "INGRESAR",
            Location = new Point(30, 242),
            Size = new Size(300, 36),
            Font = new Font("Segoe UI", 11, FontStyle.Bold),
            BackColor = Color.FromArgb(30, 80, 120),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };
        btnIngresar.FlatAppearance.BorderSize = 0;
        btnIngresar.Click += BtnIngresar_Click;
        pnlCard.Controls.Add(btnIngresar);

        // Informacion de credenciales por defecto
        var lblDefaultInfo = new Label
        {
            Text = "Usuario por defecto: admin | Contrasena: Admin123!",
            Font = new Font("Segoe UI", 8),
            ForeColor = Color.FromArgb(120, 140, 160),
            Location = new Point(40, 330),
            Size = new Size(360, 18),
            TextAlign = ContentAlignment.MiddleCenter
        };
        Controls.Add(lblDefaultInfo);

        AcceptButton = btnIngresar;
        txtUsuario.Text = "admin";
        txtContrasena.Text = "Admin123!";
        txtContrasena.Focus();
    }

    private void BtnIngresar_Click(object? sender, EventArgs e)
    {
        lblError.Text = string.Empty;
        var usuario = txtUsuario.Text.Trim();
        var contrasena = txtContrasena.Text;

        if (string.IsNullOrEmpty(usuario) || string.IsNullOrEmpty(contrasena))
        {
            lblError.Text = "Ingrese usuario y contrasena.";
            return;
        }

        btnIngresar.Enabled = false;
        btnIngresar.Text = "Ingresando...";

        if (_authService.Login(usuario, contrasena))
        {
            DialogResult = DialogResult.OK;
            Close();
        }
        else
        {
            lblError.Text = "Usuario o contrasena incorrectos.";
            txtContrasena.Clear();
            txtContrasena.Focus();
            btnIngresar.Enabled = true;
            btnIngresar.Text = "INGRESAR";
        }
    }
}
