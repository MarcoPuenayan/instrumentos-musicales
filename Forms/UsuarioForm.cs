using RegistroInstrumentos.Helpers;
using RegistroInstrumentos.Models;
using RegistroInstrumentos.Repositories;

namespace RegistroInstrumentos.Forms;

public class UsuarioForm : Form
{
    private readonly UsuarioRepository _repo;
    private DataGridView dgv = null!;
    private TextBox txtUsuario = null!, txtNombreCompleto = null!;
    private TextBox txtContrasena = null!, txtConfirmar = null!;
    private ComboBox cmbRol = null!;
    private CheckBox chkActivo = null!;
    private Button btnGuardar = null!, btnEliminar = null!;
    private int _idSeleccionado = 0;

    public UsuarioForm(UsuarioRepository repo)
    {
        _repo = repo;
        InicializarComponentes();
        CargarDatos();
    }

    private void InicializarComponentes()
    {
        Text = "Gestion de Usuarios del Sistema";
        Size = new Size(1000, 600);
        BackColor = Color.FromArgb(240, 244, 248);

        var pnlLista = new Panel { Dock = DockStyle.Left, Width = 490, Padding = new Padding(10) };
        Controls.Add(pnlLista);
        pnlLista.Controls.Add(new Label
        {
            Text = "Usuarios del Sistema", Font = new Font("Segoe UI", 14, FontStyle.Bold),
            Dock = DockStyle.Top, Height = 40, ForeColor = Color.FromArgb(30, 80, 120)
        });

        var btnNuevoLista = new Button
        {
            Text = "Nuevo Usuario", Dock = DockStyle.Top, Height = 38,
            BackColor = Color.FromArgb(30, 120, 80), ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };
        btnNuevoLista.FlatAppearance.BorderSize = 0;
        btnNuevoLista.Click += (s, e) => LimpiarFormulario();
        pnlLista.Controls.Add(btnNuevoLista);

        dgv = new DataGridView
        {
            Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect, MultiSelect = false,
            BackgroundColor = Color.White, BorderStyle = BorderStyle.None,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, RowHeadersVisible = false,
            Font = new Font("Segoe UI", 9),
            ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        };
        dgv.ColumnHeadersHeight = 32;
        dgv.EnableHeadersVisualStyles = false;
        dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 80, 120);
        dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
        dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 249, 253);
        dgv.SelectionChanged += Dgv_SelectionChanged;
        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Id", Visible = false });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "NombreUsuario", HeaderText = "Usuario" });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "NombreCompleto", HeaderText = "Nombre Completo" });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Rol", HeaderText = "Rol", FillWeight = 25 });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Activo", HeaderText = "Activo", FillWeight = 15 });
        pnlLista.Controls.Add(dgv);

        Controls.Add(new Panel { Dock = DockStyle.Left, Width = 4, BackColor = Color.FromArgb(200, 215, 230) });

        var pnlDetalle = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20) };
        Controls.Add(pnlDetalle);
        pnlDetalle.Controls.Add(new Label
        {
            Text = "Datos del Usuario", Font = new Font("Segoe UI", 13, FontStyle.Bold),
            Dock = DockStyle.Top, Height = 40, ForeColor = Color.FromArgb(30, 80, 120)
        });

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 8,
            Padding = new Padding(0, 5, 0, 0)
        };
        for (int i = 0; i < 8; i++) layout.RowStyles.Add(new RowStyle(SizeType.Percent, 12.5f));

        txtUsuario = AgregarCampo(layout, "Nombre de Usuario:", 0);
        txtNombreCompleto = AgregarCampo(layout, "Nombre Completo:", 1);

        var pnlRol = new Panel { Dock = DockStyle.Fill, Padding = new Padding(4) };
        pnlRol.Controls.Add(new Label { Text = "Rol:", Font = new Font("Segoe UI", 9, FontStyle.Bold), Dock = DockStyle.Top, Height = 22, ForeColor = Color.FromArgb(60, 80, 100) });
        cmbRol = new ComboBox { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10), DropDownStyle = ComboBoxStyle.DropDownList };
        cmbRol.Items.AddRange(new object[] { "Administrador", "Operador" });
        cmbRol.SelectedIndex = 1;
        pnlRol.Controls.Add(cmbRol);
        layout.Controls.Add(pnlRol, 0, 2);

        var pnlActivo = new Panel { Dock = DockStyle.Fill, Padding = new Padding(4) };
        chkActivo = new CheckBox
        {
            Text = "Usuario Activo", Font = new Font("Segoe UI", 10),
            Dock = DockStyle.Fill, Checked = true
        };
        pnlActivo.Controls.Add(chkActivo);
        layout.Controls.Add(pnlActivo, 0, 3);

        var pnlSep = new Panel { Dock = DockStyle.Fill, Padding = new Padding(4) };
        pnlSep.Controls.Add(new Label
        {
            Text = "--- Contrasena (dejar en blanco para no cambiar) ---",
            Font = new Font("Segoe UI", 9, FontStyle.Italic), ForeColor = Color.Gray, Dock = DockStyle.Fill
        });
        layout.Controls.Add(pnlSep, 0, 4);

        txtContrasena = AgregarCampoPassword(layout, "Nueva Contrasena:", 5);
        txtConfirmar = AgregarCampoPassword(layout, "Confirmar Contrasena:", 6);

        var pnlBts = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom, Height = 55,
            FlowDirection = FlowDirection.RightToLeft, Padding = new Padding(5)
        };
        btnEliminar = new Button
        {
            Text = "Desactivar", Width = 110, Height = 38,
            BackColor = Color.FromArgb(200, 60, 60), ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat, Enabled = false
        };
        btnEliminar.FlatAppearance.BorderSize = 0;
        btnEliminar.Click += BtnEliminar_Click;

        btnGuardar = new Button
        {
            Text = "Guardar", Width = 120, Height = 38,
            BackColor = Color.FromArgb(30, 80, 120), ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };
        btnGuardar.FlatAppearance.BorderSize = 0;
        btnGuardar.Click += BtnGuardar_Click;

        var btnLimpiar = new Button
        {
            Text = "Limpiar", Width = 100, Height = 38,
            BackColor = Color.FromArgb(100, 120, 140), ForeColor = Color.White, FlatStyle = FlatStyle.Flat
        };
        btnLimpiar.FlatAppearance.BorderSize = 0;
        btnLimpiar.Click += (s, e) => LimpiarFormulario();

        pnlBts.Controls.AddRange(new Control[] { btnEliminar, btnGuardar, btnLimpiar });
        pnlDetalle.Controls.Add(pnlBts);
        pnlDetalle.Controls.Add(layout);
    }

    private TextBox AgregarCampo(TableLayoutPanel layout, string etiqueta, int row)
    {
        var pnl = new Panel { Dock = DockStyle.Fill, Padding = new Padding(4) };
        pnl.Controls.Add(new Label
        {
            Text = etiqueta, Font = new Font("Segoe UI", 9, FontStyle.Bold),
            Dock = DockStyle.Top, Height = 22, ForeColor = Color.FromArgb(60, 80, 100)
        });
        var txt = new TextBox { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10), BorderStyle = BorderStyle.FixedSingle };
        pnl.Controls.Add(txt);
        layout.Controls.Add(pnl, 0, row);
        return txt;
    }

    private TextBox AgregarCampoPassword(TableLayoutPanel layout, string etiqueta, int row)
    {
        var txt = AgregarCampo(layout, etiqueta, row);
        txt.PasswordChar = '*';
        return txt;
    }

    private void CargarDatos()
    {
        var datos = _repo.ObtenerTodos();
        dgv.Rows.Clear();
        foreach (var u in datos)
            dgv.Rows.Add(u.Id, u.NombreUsuario, u.NombreCompleto, u.Rol, u.Activo ? "Si" : "No");
    }

    private void Dgv_SelectionChanged(object? sender, EventArgs e)
    {
        if (dgv.SelectedRows.Count == 0) return;
        var id = (int)dgv.SelectedRows[0].Cells["Id"].Value;
        var u = _repo.ObtenerPorId(id);
        if (u == null) return;
        _idSeleccionado = u.Id;
        txtUsuario.Text = u.NombreUsuario;
        txtNombreCompleto.Text = u.NombreCompleto;
        cmbRol.SelectedItem = u.Rol;
        chkActivo.Checked = u.Activo;
        txtContrasena.Text = txtConfirmar.Text = string.Empty;
        btnEliminar.Enabled = true;
    }

    private void BtnGuardar_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtUsuario.Text))
        {
            MessageBox.Show("El nombre de usuario es requerido.", "Validacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        if (!string.IsNullOrEmpty(txtContrasena.Text))
        {
            if (txtContrasena.Text != txtConfirmar.Text)
            {
                MessageBox.Show("Las contrasenas no coinciden.", "Validacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (txtContrasena.Text.Length < 6)
            {
                MessageBox.Show("La contrasena debe tener al menos 6 caracteres.", "Validacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
        }

        if (_idSeleccionado == 0)
        {
            if (string.IsNullOrEmpty(txtContrasena.Text))
            {
                MessageBox.Show("La contrasena es requerida para nuevos usuarios.", "Validacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (_repo.ObtenerPorNombreUsuario(txtUsuario.Text.Trim()) != null)
            {
                MessageBox.Show("Ya existe un usuario con ese nombre.", "Duplicado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            _repo.Agregar(new Usuario
            {
                NombreUsuario = txtUsuario.Text.Trim(),
                Contrasena = PasswordHelper.HashPassword(txtContrasena.Text),
                NombreCompleto = txtNombreCompleto.Text.Trim(),
                Rol = cmbRol.SelectedItem?.ToString() ?? "Operador",
                Activo = chkActivo.Checked
            });
        }
        else
        {
            var u = _repo.ObtenerPorId(_idSeleccionado)!;
            u.NombreUsuario = txtUsuario.Text.Trim();
            u.NombreCompleto = txtNombreCompleto.Text.Trim();
            u.Rol = cmbRol.SelectedItem?.ToString() ?? "Operador";
            u.Activo = chkActivo.Checked;
            if (!string.IsNullOrEmpty(txtContrasena.Text))
                u.Contrasena = PasswordHelper.HashPassword(txtContrasena.Text);
            _repo.Actualizar(u);
        }
        MessageBox.Show("Usuario guardado correctamente.", "Exito", MessageBoxButtons.OK, MessageBoxIcon.Information);
        LimpiarFormulario();
        CargarDatos();
    }

    private void BtnEliminar_Click(object? sender, EventArgs e)
    {
        if (_idSeleccionado == 0) return;
        if (MessageBox.Show("¿Desactivar este usuario?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        {
            _repo.Eliminar(_idSeleccionado);
            LimpiarFormulario();
            CargarDatos();
        }
    }

    private void LimpiarFormulario()
    {
        _idSeleccionado = 0;
        txtUsuario.Text = txtNombreCompleto.Text = txtContrasena.Text = txtConfirmar.Text = string.Empty;
        cmbRol.SelectedIndex = 1;
        chkActivo.Checked = true;
        btnEliminar.Enabled = false;
        dgv.ClearSelection();
    }
}
