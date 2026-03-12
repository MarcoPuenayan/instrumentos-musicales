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
        Size = new Size(1100, 650);
        BackColor = Color.FromArgb(240, 244, 248);

        var pnlLista = new Panel { Dock = DockStyle.Left, Width = 540, Padding = new Padding(10) };
        Controls.Add(pnlLista);

        pnlLista.Controls.Add(new Label
        {
            Text = "Registro de Personas", Font = new Font("Segoe UI", 14, FontStyle.Bold),
            Dock = DockStyle.Top, Height = 40, ForeColor = Color.FromArgb(30, 80, 120)
        });

        var pnlBuscar = new Panel { Dock = DockStyle.Top, Height = 40 };
        txtBuscar = new TextBox
        {
            Dock = DockStyle.Fill, Font = new Font("Segoe UI", 11),
            PlaceholderText = "Buscar por nombre, apellido o cedula..."
        };
        txtBuscar.TextChanged += (s, e) => CargarDatos(txtBuscar.Text);
        var btnNuevolista = new Button
        {
            Text = "Nueva Persona", Dock = DockStyle.Right, Width = 130,
            BackColor = Color.FromArgb(30, 120, 80), ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9, FontStyle.Bold)
        };
        btnNuevolista.FlatAppearance.BorderSize = 0;
        btnNuevolista.Click += (s, e) => LimpiarFormulario();
        pnlBuscar.Controls.Add(txtBuscar);
        pnlBuscar.Controls.Add(btnNuevolista);
        pnlLista.Controls.Add(pnlBuscar);

        dgv = new DataGridView
        {
            Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect, MultiSelect = false,
            BackgroundColor = Color.White, BorderStyle = BorderStyle.None,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            RowHeadersVisible = false, Font = new Font("Segoe UI", 9),
            ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        };
        dgv.ColumnHeadersHeight = 32;
        dgv.EnableHeadersVisualStyles = false;
        dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 80, 120);
        dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
        dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 249, 253);
        dgv.SelectionChanged += Dgv_SelectionChanged;
        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Id", HeaderText = "ID", Visible = false });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Cedula", HeaderText = "Cedula", FillWeight = 20 });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Nombres", HeaderText = "Nombres" });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Apellidos", HeaderText = "Apellidos" });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Departamento", HeaderText = "Departamento" });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Telefono", HeaderText = "Telefono", FillWeight = 20 });
        pnlLista.Controls.Add(dgv);

        Controls.Add(new Panel { Dock = DockStyle.Left, Width = 4, BackColor = Color.FromArgb(200, 215, 230) });

        var pnlDetalle = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20) };
        Controls.Add(pnlDetalle);
        pnlDetalle.Controls.Add(new Label
        {
            Text = "Datos de la Persona", Font = new Font("Segoe UI", 13, FontStyle.Bold),
            Dock = DockStyle.Top, Height = 40, ForeColor = Color.FromArgb(30, 80, 120)
        });

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 5,
            Padding = new Padding(0, 10, 0, 0)
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        for (int i = 0; i < 5; i++) layout.RowStyles.Add(new RowStyle(SizeType.Percent, 20));

        txtCedula = AgregarCampo(layout, "Cedula / Identificacion:", 0, 0);
        txtNombres = AgregarCampo(layout, "Nombres:", 0, 1);
        txtApellidos = AgregarCampo(layout, "Apellidos:", 1, 1);
        txtTelefono = AgregarCampo(layout, "Telefono:", 0, 2);
        txtEmail = AgregarCampo(layout, "Email:", 1, 2);
        txtDepartamento = AgregarCampo(layout, "Departamento / Area:", 0, 3);
        txtDireccion = AgregarCampo(layout, "Direccion:", 1, 3);

        var pnlBts = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom, Height = 55,
            FlowDirection = FlowDirection.RightToLeft, Padding = new Padding(5)
        };
        btnEliminar = new Button
        {
            Text = "Eliminar", Width = 110, Height = 38,
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

    private TextBox AgregarCampo(TableLayoutPanel layout, string etiqueta, int col, int row)
    {
        var pnl = new Panel { Dock = DockStyle.Fill, Padding = new Padding(4) };
        pnl.Controls.Add(new Label
        {
            Text = etiqueta, Font = new Font("Segoe UI", 9, FontStyle.Bold),
            Dock = DockStyle.Top, Height = 22, ForeColor = Color.FromArgb(60, 80, 100)
        });
        var txt = new TextBox
        {
            Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10), BorderStyle = BorderStyle.FixedSingle
        };
        pnl.Controls.Add(txt);
        layout.Controls.Add(pnl, col, row);
        return txt;
    }

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
        var persona = _repo.ObtenerPorId(id);
        if (persona == null) return;
        _idSeleccionado = persona.Id;
        txtCedula.Text = persona.Cedula;
        txtNombres.Text = persona.Nombres;
        txtApellidos.Text = persona.Apellidos;
        txtTelefono.Text = persona.Telefono;
        txtEmail.Text = persona.Email;
        txtDireccion.Text = persona.Direccion;
        txtDepartamento.Text = persona.Departamento;
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
    }
}
