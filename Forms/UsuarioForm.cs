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
        Size = new Size(1050, 640);
        MinimumSize = new Size(850, 480);
        BackColor = Color.FromArgb(240, 244, 248);

        // SplitContainer: Panel1=form (top), Panel2=list (bottom)
        var split = new SplitContainer
        {
            Dock = DockStyle.Fill, Orientation = Orientation.Horizontal,
            SplitterDistance = 460, SplitterWidth = 4,
            Panel1MinSize = 200, Panel2MinSize = 150,
            BackColor = Color.FromArgb(200, 215, 230)
        };
        Controls.Add(split);
        var pnlDetalle = split.Panel1;
        pnlDetalle.Padding = new Padding(20, 12, 20, 10);

        var lblForm = new Label { Text = "Datos del Usuario", Font = new Font("Segoe UI", 13, FontStyle.Bold), Dock = DockStyle.Top, Height = 38, ForeColor = Color.FromArgb(30, 80, 120) };
        var pnlBts = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 52, FlowDirection = FlowDirection.RightToLeft, Padding = new Padding(0, 7, 0, 0) };
        btnEliminar = Btn("Desactivar", Color.FromArgb(200, 60, 60)); btnEliminar.Enabled = false; btnEliminar.Click += BtnEliminar_Click;
        btnGuardar = Btn("Guardar", Color.FromArgb(30, 80, 120), true); btnGuardar.Click += BtnGuardar_Click;
        var btnLimpiar = Btn("Limpiar", Color.FromArgb(100, 120, 140)); btnLimpiar.Click += (s, e) => LimpiarFormulario();
        pnlBts.Controls.AddRange(new Control[] { btnEliminar, btnGuardar, btnLimpiar });

        var layout = CrearLayout(1, 7, 55);
        txtUsuario = Campo(layout, "Nombre de Usuario:", 0, 0);
        txtNombreCompleto = Campo(layout, "Nombre Completo:", 0, 1);

        // Rol
        var flowRol = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, WrapContents = false, Padding = new Padding(4, 4, 4, 2) };
        var lblRol = new Label { Text = "Rol:", Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = Color.FromArgb(55, 75, 105), AutoSize = false, Height = 20, Width = 400 };
        cmbRol = new ComboBox { Font = new Font("Segoe UI", 10), DropDownStyle = ComboBoxStyle.DropDownList, Height = 28, Width = 400 };
        cmbRol.Items.AddRange(new object[] { "Administrador", "Operador" }); cmbRol.SelectedIndex = 1;
        flowRol.Controls.Add(lblRol); flowRol.Controls.Add(cmbRol);
        flowRol.SizeChanged += (_, _) => { int w = flowRol.ClientSize.Width - 10; if (w > 0) { lblRol.Width = w; cmbRol.Width = w; } };
        layout.Controls.Add(flowRol, 0, 2);

        // Activo
        var flowActivo = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, WrapContents = false, Padding = new Padding(4, 14, 4, 2) };
        chkActivo = new CheckBox { Text = "  Usuario Activo", Font = new Font("Segoe UI", 10), Checked = true, Height = 28 };
        flowActivo.Controls.Add(chkActivo);
        layout.Controls.Add(flowActivo, 0, 3);

        // Separador de seccion contrasena
        var flowSep = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, WrapContents = false, Padding = new Padding(4, 18, 4, 2) };
        flowSep.Controls.Add(new Label { Text = "Contrasena — dejar en blanco para no modificar:", Font = new Font("Segoe UI", 9, FontStyle.Italic), ForeColor = Color.FromArgb(110, 120, 140), AutoSize = false, Height = 22, Width = 400 });
        layout.Controls.Add(flowSep, 0, 4);

        txtContrasena = Campo(layout, "Nueva Contrasena:", 0, 5); txtContrasena.PasswordChar = '*';
        txtConfirmar = Campo(layout, "Confirmar Contrasena:", 0, 6); txtConfirmar.PasswordChar = '*';

        pnlDetalle.Controls.Add(layout);
        pnlDetalle.Controls.Add(pnlBts);
        pnlDetalle.Controls.Add(lblForm);

        // ── Panel inferior: lista ────────────────────────────────────────────
        var pnlLista = split.Panel2;
        pnlLista.Padding = new Padding(10);

        var lblTitulo = new Label { Text = "Usuarios del Sistema", Font = new Font("Segoe UI", 13, FontStyle.Bold), Dock = DockStyle.Top, Height = 40, ForeColor = Color.FromArgb(30, 80, 120) };
        var btnNuevo = new Button { Text = "Nuevo Usuario", Dock = DockStyle.Top, Height = 38, BackColor = Color.FromArgb(30, 120, 80), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
        btnNuevo.FlatAppearance.BorderSize = 0;
        btnNuevo.Click += (s, e) => LimpiarFormulario();

        dgv = CrearDgv();
        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Id", Visible = false });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "NombreUsuario", HeaderText = "Usuario" });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "NombreCompleto", HeaderText = "Nombre Completo" });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Rol", HeaderText = "Rol", FillWeight = 25 });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Activo", HeaderText = "Activo", FillWeight = 14 });
        dgv.SelectionChanged += Dgv_SelectionChanged;

        pnlLista.Controls.Add(dgv);
        pnlLista.Controls.Add(btnNuevo);
        pnlLista.Controls.Add(lblTitulo);
    }

    private static DataGridView CrearDgv()
    {
        var d = new DataGridView
        {
            Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect, MultiSelect = false,
            BackgroundColor = Color.White, BorderStyle = BorderStyle.None,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, RowHeadersVisible = false,
            Font = new Font("Segoe UI", 9), ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
            CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal, GridColor = Color.FromArgb(225, 235, 245)
        };
        d.ColumnHeadersHeight = 33; d.EnableHeadersVisualStyles = false;
        d.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 80, 120);
        d.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        d.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
        d.ColumnHeadersDefaultCellStyle.Padding = new Padding(4, 0, 0, 0);
        d.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(246, 250, 254);
        d.DefaultCellStyle.Padding = new Padding(4, 0, 0, 0); d.RowTemplate.Height = 30;
        return d;
    }

    private static TableLayoutPanel CrearLayout(int cols, int rows, int rh)
    {
        var t = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = cols, RowCount = rows, Padding = new Padding(0, 6, 0, 0) };
        for (int c = 0; c < cols; c++) t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / cols));
        for (int r = 0; r < rows; r++) t.RowStyles.Add(new RowStyle(SizeType.Absolute, rh));
        return t;
    }

    private static TextBox Campo(TableLayoutPanel layout, string etiq, int col, int row)
    {
        var flow = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, WrapContents = false, Padding = new Padding(4, 4, 4, 2) };
        var lbl = new Label { Text = etiq, Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = Color.FromArgb(55, 75, 105), AutoSize = false, Height = 20, Width = 400 };
        var txt = new TextBox { Font = new Font("Segoe UI", 10), BorderStyle = BorderStyle.FixedSingle, BackColor = Color.White, Height = 28, Width = 400 };
        flow.Controls.Add(lbl); flow.Controls.Add(txt);
        flow.SizeChanged += (_, _) => { int w = flow.ClientSize.Width - 10; if (w > 0) { lbl.Width = w; txt.Width = w; } };
        layout.Controls.Add(flow, col, row); return txt;
    }

    private static Button Btn(string texto, Color color, bool bold = false)
    {
        var b = new Button { Text = texto, Width = 115, Height = 38, BackColor = color, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand, Font = bold ? new Font("Segoe UI", 10, FontStyle.Bold) : new Font("Segoe UI", 9) };
        b.FlatAppearance.BorderSize = 0; return b;
    }

    private void CargarDatos()
    {
        var datos = _repo.ObtenerTodos(); dgv.Rows.Clear();
        foreach (var u in datos) dgv.Rows.Add(u.Id, u.NombreUsuario, u.NombreCompleto, u.Rol, u.Activo ? "Si" : "No");
    }

    private void Dgv_SelectionChanged(object? sender, EventArgs e)
    {
        if (dgv.SelectedRows.Count == 0) return;
        var id = (int)dgv.SelectedRows[0].Cells["Id"].Value;
        var u = _repo.ObtenerPorId(id); if (u == null) return;
        _idSeleccionado = u.Id;
        txtUsuario.Text = u.NombreUsuario; txtNombreCompleto.Text = u.NombreCompleto;
        cmbRol.SelectedItem = u.Rol; chkActivo.Checked = u.Activo;
        txtContrasena.Text = txtConfirmar.Text = string.Empty;
        btnEliminar.Enabled = true;
    }

    private void BtnGuardar_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtUsuario.Text)) { MessageBox.Show("El nombre de usuario es requerido.", "Validacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
        if (!string.IsNullOrEmpty(txtContrasena.Text))
        {
            if (txtContrasena.Text != txtConfirmar.Text) { MessageBox.Show("Las contrasenas no coinciden.", "Validacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (txtContrasena.Text.Length < 6) { MessageBox.Show("La contrasena debe tener al menos 6 caracteres.", "Validacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
        }
        if (_idSeleccionado == 0)
        {
            if (string.IsNullOrEmpty(txtContrasena.Text)) { MessageBox.Show("La contrasena es requerida para nuevos usuarios.", "Validacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (_repo.ObtenerPorNombreUsuario(txtUsuario.Text.Trim()) != null) { MessageBox.Show("Ya existe un usuario con ese nombre.", "Duplicado", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            _repo.Agregar(new Usuario { NombreUsuario = txtUsuario.Text.Trim(), Contrasena = PasswordHelper.HashPassword(txtContrasena.Text), NombreCompleto = txtNombreCompleto.Text.Trim(), Rol = cmbRol.SelectedItem?.ToString() ?? "Operador", Activo = chkActivo.Checked });
        }
        else
        {
            var u = _repo.ObtenerPorId(_idSeleccionado)!;
            u.NombreUsuario = txtUsuario.Text.Trim(); u.NombreCompleto = txtNombreCompleto.Text.Trim();
            u.Rol = cmbRol.SelectedItem?.ToString() ?? "Operador"; u.Activo = chkActivo.Checked;
            if (!string.IsNullOrEmpty(txtContrasena.Text)) u.Contrasena = PasswordHelper.HashPassword(txtContrasena.Text);
            _repo.Actualizar(u);
        }
        MessageBox.Show("Usuario guardado correctamente.", "Exito", MessageBoxButtons.OK, MessageBoxIcon.Information);
        LimpiarFormulario(); CargarDatos();
    }

    private void BtnEliminar_Click(object? sender, EventArgs e)
    {
        if (_idSeleccionado == 0) return;
        if (MessageBox.Show("¿Desactivar este usuario?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        { _repo.Eliminar(_idSeleccionado); LimpiarFormulario(); CargarDatos(); }
    }

    private void LimpiarFormulario()
    {
        _idSeleccionado = 0;
        txtUsuario.Text = txtNombreCompleto.Text = txtContrasena.Text = txtConfirmar.Text = string.Empty;
        cmbRol.SelectedIndex = 1; chkActivo.Checked = true;
        btnEliminar.Enabled = false; dgv.ClearSelection(); txtUsuario.Focus();
    }
}
