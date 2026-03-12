using RegistroInstrumentos.Models;
using RegistroInstrumentos.Repositories;

namespace RegistroInstrumentos.Forms;

public class PersonaForm : Form
{
    private readonly PersonaRepository _repo;
    private DataGridView dgv = null!;
    private TextBox txtBuscar = null!;
    private TextBox txtCedula = null!, txtNombres = null!, txtApellidos = null!;
    private TextBox txtTelefono = null!, txtEmail = null!, txtDireccion = null!, txtDepartamento = null!;
    private Button btnGuardar = null!, btnEliminar = null!;
    private int _idSeleccionado = 0;

    public PersonaForm(PersonaRepository repo)
    {
        _repo = repo;
        InicializarComponentes();
        CargarDatos();
    }

    private void InicializarComponentes()
    {
        Text = "Gestion de Personas";
        Size = new Size(1000, 790);
        MinimumSize = new Size(800, 620);
        BackColor = Color.FromArgb(240, 244, 248);

        // SplitContainer: Panel1=form (top), Panel2=list (bottom)
        var split = new SplitContainer
        {
            Dock = DockStyle.Fill, Orientation = Orientation.Horizontal,
            SplitterDistance = 380, SplitterWidth = 4,
            Panel1MinSize = 200, Panel2MinSize = 150,
            BackColor = Color.FromArgb(200, 215, 230)
        };
        Controls.Add(split);
        var pnlDetalle = split.Panel1;
        pnlDetalle.Padding = new Padding(20, 15, 20, 10);

        var lblTitForm = new Label
        {
            Text = "Datos de la Persona", Font = new Font("Segoe UI", 13, FontStyle.Bold),
            Dock = DockStyle.Top, Height = 38, ForeColor = Color.FromArgb(30, 80, 120)
        };

        var pnlBts = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom, Height = 52,
            FlowDirection = FlowDirection.RightToLeft, Padding = new Padding(0, 7, 0, 0)
        };
        btnEliminar = CrearBoton("Eliminar", Color.FromArgb(200, 60, 60));
        btnEliminar.Enabled = false;
        btnEliminar.Click += BtnEliminar_Click;
        btnGuardar = CrearBoton("Guardar", Color.FromArgb(30, 80, 120), bold: true);
        btnGuardar.Click += BtnGuardar_Click;
        var btnLimpiar = CrearBoton("Limpiar", Color.FromArgb(100, 120, 140));
        btnLimpiar.Click += (s, e) => LimpiarFormulario();
        pnlBts.Controls.AddRange(new Control[] { btnEliminar, btnGuardar, btnLimpiar });

        var layout = CrearLayout(columnas: 2, filas: 4, altoFila: 72);
        txtCedula = AgregarCampo(layout, "Cedula / Identificacion:", 0, 0, colSpan: 2);
        txtNombres = AgregarCampo(layout, "Nombres:", 0, 1);
        txtApellidos = AgregarCampo(layout, "Apellidos:", 1, 1);
        txtTelefono = AgregarCampo(layout, "Telefono:", 0, 2);
        txtEmail = AgregarCampo(layout, "Email:", 1, 2);
        txtDepartamento = AgregarCampo(layout, "Departamento / Area:", 0, 3);
        txtDireccion = AgregarCampo(layout, "Direccion:", 1, 3);

        pnlDetalle.Controls.Add(layout);
        pnlDetalle.Controls.Add(pnlBts);
        pnlDetalle.Controls.Add(lblTitForm);

        // ── Panel inferior (lista) ─────────────────────────────────────────
        var pnlLista = split.Panel2;
        pnlLista.Padding = new Padding(10);

        var lblTitLista = new Label
        {
            Text = "Registro de Personas", Font = new Font("Segoe UI", 14, FontStyle.Bold),
            Dock = DockStyle.Top, Height = 40, ForeColor = Color.FromArgb(30, 80, 120)
        };

        var pnlBuscar = new Panel { Dock = DockStyle.Top, Height = 42 };
        txtBuscar = new TextBox
        {
            Dock = DockStyle.Fill, Font = new Font("Segoe UI", 11),
            PlaceholderText = "Buscar por nombre, apellido o cedula..."
        };
        txtBuscar.TextChanged += (s, e) => CargarDatos(txtBuscar.Text);
        var btnNuevoLista = new Button
        {
            Text = "Nueva Persona", Dock = DockStyle.Right, Width = 130,
            BackColor = Color.FromArgb(30, 120, 80), ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9, FontStyle.Bold)
        };
        btnNuevoLista.FlatAppearance.BorderSize = 0;
        btnNuevoLista.Click += (s, e) => LimpiarFormulario();
        pnlBuscar.Controls.Add(txtBuscar);
        pnlBuscar.Controls.Add(btnNuevoLista);

        dgv = CrearDataGridView();
        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Id", Visible = false });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Cedula", HeaderText = "Cedula", FillWeight = 22 });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Nombres", HeaderText = "Nombres" });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Apellidos", HeaderText = "Apellidos" });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Departamento", HeaderText = "Departamento" });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Telefono", HeaderText = "Telefono", FillWeight = 22 });
        dgv.SelectionChanged += Dgv_SelectionChanged;

        pnlLista.Controls.Add(dgv);
        pnlLista.Controls.Add(pnlBuscar);
        pnlLista.Controls.Add(lblTitLista);
    }

    // ── Helpers de UI ──────────────────────────────────────────────────────

    private static DataGridView CrearDataGridView()
    {
        var dgv = new DataGridView
        {
            Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect, MultiSelect = false,
            BackgroundColor = Color.White, BorderStyle = BorderStyle.None,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, RowHeadersVisible = false,
            Font = new Font("Segoe UI", 9),
            ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
            CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
            GridColor = Color.FromArgb(225, 235, 245)
        };
        dgv.ColumnHeadersHeight = 33;
        dgv.EnableHeadersVisualStyles = false;
        dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 80, 120);
        dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
        dgv.ColumnHeadersDefaultCellStyle.Padding = new Padding(4, 0, 0, 0);
        dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(246, 250, 254);
        dgv.DefaultCellStyle.Padding = new Padding(4, 0, 0, 0);
        dgv.RowTemplate.Height = 30;
        return dgv;
    }

    private static TableLayoutPanel CrearLayout(int columnas, int filas, int altoFila)
    {
        var tlp = new TableLayoutPanel
        {
            Dock = DockStyle.Fill, ColumnCount = columnas, RowCount = filas,
            Padding = new Padding(0, 8, 0, 0), CellBorderStyle = TableLayoutPanelCellBorderStyle.None
        };
        for (int c = 0; c < columnas; c++)
            tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / columnas));
        for (int r = 0; r < filas; r++)
            tlp.RowStyles.Add(new RowStyle(SizeType.Absolute, altoFila));
        return tlp;
    }

    private static TextBox AgregarCampo(TableLayoutPanel layout, string etiqueta,
        int col, int row, int colSpan = 1)
    {
        var flow = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown,
            WrapContents = false, Padding = new Padding(4, 4, 4, 2)
        };
        var lbl = new Label
        {
            Text = etiqueta, Font = new Font("Segoe UI", 9, FontStyle.Bold),
            ForeColor = Color.FromArgb(55, 75, 105), AutoSize = false, Height = 20, Width = 400
        };
        var txt = new TextBox
        {
            Font = new Font("Segoe UI", 10), BorderStyle = BorderStyle.FixedSingle,
            BackColor = Color.White, Height = 28, Width = 400
        };
        flow.Controls.Add(lbl);
        flow.Controls.Add(txt);
        flow.SizeChanged += (_, _) =>
        {
            int w = flow.ClientSize.Width - 10;
            if (w > 0) { lbl.Width = w; txt.Width = w; }
        };
        if (colSpan > 1) layout.SetColumnSpan(flow, colSpan);
        layout.Controls.Add(flow, col, row);
        return txt;
    }

    private static Button CrearBoton(string texto, Color color, bool bold = false)
    {
        var btn = new Button
        {
            Text = texto, Width = 110, Height = 38,
            BackColor = color, ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = bold ? new Font("Segoe UI", 10, FontStyle.Bold) : new Font("Segoe UI", 9),
            Cursor = Cursors.Hand
        };
        btn.FlatAppearance.BorderSize = 0;
        return btn;
    }

    // ── Logica ─────────────────────────────────────────────────────────────

    private void CargarDatos(string buscar = "")
    {
        var datos = string.IsNullOrEmpty(buscar)
            ? _repo.ObtenerTodos()
            : _repo.BuscarPorNombre(buscar);
        dgv.Rows.Clear();
        foreach (var p in datos)
            dgv.Rows.Add(p.Id, p.Cedula, p.Nombres, p.Apellidos, p.Departamento, p.Telefono);
    }

    private void Dgv_SelectionChanged(object? sender, EventArgs e)
    {
        if (dgv.SelectedRows.Count == 0) return;
        var id = (int)dgv.SelectedRows[0].Cells["Id"].Value;
        var p = _repo.ObtenerPorId(id);
        if (p == null) return;
        _idSeleccionado = p.Id;
        txtCedula.Text = p.Cedula; txtNombres.Text = p.Nombres; txtApellidos.Text = p.Apellidos;
        txtTelefono.Text = p.Telefono; txtEmail.Text = p.Email;
        txtDireccion.Text = p.Direccion; txtDepartamento.Text = p.Departamento;
        btnEliminar.Enabled = true;
    }

    private void BtnGuardar_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtCedula.Text) || string.IsNullOrWhiteSpace(txtNombres.Text))
        {
            MessageBox.Show("Cedula y Nombres son requeridos.", "Validacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        if (_idSeleccionado == 0)
        {
            if (_repo.ObtenerPorCedula(txtCedula.Text.Trim()) != null)
            {
                MessageBox.Show("Ya existe una persona con esa cedula.", "Duplicado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            _repo.Agregar(new Persona
            {
                Cedula = txtCedula.Text.Trim(), Nombres = txtNombres.Text.Trim(),
                Apellidos = txtApellidos.Text.Trim(), Telefono = txtTelefono.Text.Trim(),
                Email = txtEmail.Text.Trim(), Direccion = txtDireccion.Text.Trim(),
                Departamento = txtDepartamento.Text.Trim()
            });
        }
        else
        {
            var p = _repo.ObtenerPorId(_idSeleccionado)!;
            p.Cedula = txtCedula.Text.Trim(); p.Nombres = txtNombres.Text.Trim();
            p.Apellidos = txtApellidos.Text.Trim(); p.Telefono = txtTelefono.Text.Trim();
            p.Email = txtEmail.Text.Trim(); p.Direccion = txtDireccion.Text.Trim();
            p.Departamento = txtDepartamento.Text.Trim();
            _repo.Actualizar(p);
        }
        MessageBox.Show("Guardado correctamente.", "Exito", MessageBoxButtons.OK, MessageBoxIcon.Information);
        LimpiarFormulario();
        CargarDatos();
    }

    private void BtnEliminar_Click(object? sender, EventArgs e)
    {
        if (_idSeleccionado == 0) return;
        if (MessageBox.Show("¿Eliminar esta persona?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        {
            _repo.Eliminar(_idSeleccionado);
            LimpiarFormulario();
            CargarDatos();
        }
    }

    private void LimpiarFormulario()
    {
        _idSeleccionado = 0;
        txtCedula.Text = txtNombres.Text = txtApellidos.Text = txtTelefono.Text =
        txtEmail.Text = txtDireccion.Text = txtDepartamento.Text = string.Empty;
        btnEliminar.Enabled = false;
        dgv.ClearSelection();
        txtCedula.Focus();
    }
}
