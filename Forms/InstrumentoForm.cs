using RegistroInstrumentos.Models;
using RegistroInstrumentos.Repositories;

namespace RegistroInstrumentos.Forms;

public class InstrumentoForm : Form
{
    private readonly InstrumentoRepository _repo;
    private DataGridView dgv = null!;
    private TextBox txtBuscar = null!;
    private int _idSeleccionado = 0;
    private TextBox txtCodigo = null!, txtNombre = null!, txtMarca = null!, txtModelo = null!;
    private TextBox txtNumeroSerie = null!, txtDescripcion = null!;
    private NumericUpDown numValor = null!;
    private DateTimePicker dtpAdquisicion = null!;
    private ComboBox cmbTipo = null!, cmbEstado = null!;
    private Button btnGuardar = null!, btnEliminar = null!;

    public InstrumentoForm(InstrumentoRepository repo)
    {
        _repo = repo;
        InicializarComponentes();
        CargarDatos();
    }

    private void InicializarComponentes()
    {
        Text = "Inventario de Instrumentos Musicales";
        Size = new Size(1150, 680);
        BackColor = Color.FromArgb(240, 244, 248);

        var pnlLista = new Panel { Dock = DockStyle.Left, Width = 545, Padding = new Padding(10) };
        Controls.Add(pnlLista);
        pnlLista.Controls.Add(new Label
        {
            Text = "Inventario de Instrumentos", Font = new Font("Segoe UI", 14, FontStyle.Bold),
            Dock = DockStyle.Top, Height = 40, ForeColor = Color.FromArgb(30, 80, 120)
        });

        var pnlBuscar = new Panel { Dock = DockStyle.Top, Height = 40 };
        txtBuscar = new TextBox
        {
            Dock = DockStyle.Fill, Font = new Font("Segoe UI", 11),
            PlaceholderText = "Buscar por nombre, codigo o marca..."
        };
        txtBuscar.TextChanged += (s, e) => CargarDatos(txtBuscar.Text);
        var btnNuevo = new Button
        {
            Text = "Nuevo", Dock = DockStyle.Right, Width = 90,
            BackColor = Color.FromArgb(30, 120, 80), ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9, FontStyle.Bold)
        };
        btnNuevo.FlatAppearance.BorderSize = 0;
        btnNuevo.Click += (s, e) => LimpiarFormulario();
        pnlBuscar.Controls.Add(txtBuscar);
        pnlBuscar.Controls.Add(btnNuevo);
        pnlLista.Controls.Add(pnlBuscar);

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
        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Id", HeaderText = "ID", Visible = false });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Codigo", HeaderText = "Codigo", FillWeight = 18 });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Nombre", HeaderText = "Nombre" });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Tipo", HeaderText = "Tipo", FillWeight = 18 });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Marca", HeaderText = "Marca", FillWeight = 18 });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Estado", HeaderText = "Estado", FillWeight = 20 });
        pnlLista.Controls.Add(dgv);

        Controls.Add(new Panel { Dock = DockStyle.Left, Width = 4, BackColor = Color.FromArgb(200, 215, 230) });

        var pnlDetalle = new Panel { Dock = DockStyle.Fill, Padding = new Padding(15) };
        Controls.Add(pnlDetalle);
        pnlDetalle.Controls.Add(new Label
        {
            Text = "Datos del Instrumento", Font = new Font("Segoe UI", 13, FontStyle.Bold),
            Dock = DockStyle.Top, Height = 40, ForeColor = Color.FromArgb(30, 80, 120)
        });

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 6,
            Padding = new Padding(0, 5, 0, 0)
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        for (int i = 0; i < 6; i++) layout.RowStyles.Add(new RowStyle(SizeType.Percent, 16.6f));

        txtCodigo = AgregarCampo(layout, "Codigo:", 0, 0);
        txtNombre = AgregarCampo(layout, "Nombre:", 1, 0);

        var pnlTipo = new Panel { Dock = DockStyle.Fill, Padding = new Padding(4) };
        pnlTipo.Controls.Add(new Label { Text = "Tipo:", Font = new Font("Segoe UI", 9, FontStyle.Bold), Dock = DockStyle.Top, Height = 22, ForeColor = Color.FromArgb(60, 80, 100) });
        cmbTipo = new ComboBox { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10), DropDownStyle = ComboBoxStyle.DropDownList };
        cmbTipo.Items.AddRange(new object[] { "Cuerda", "Viento Madera", "Viento Metal", "Percusion", "Teclado", "Electronico", "Otro" });
        cmbTipo.SelectedIndex = 0;
        pnlTipo.Controls.Add(cmbTipo);
        layout.Controls.Add(pnlTipo, 0, 1);

        txtMarca = AgregarCampo(layout, "Marca:", 1, 1);
        txtModelo = AgregarCampo(layout, "Modelo:", 0, 2);
        txtNumeroSerie = AgregarCampo(layout, "Numero de Serie:", 1, 2);

        var pnlValor = new Panel { Dock = DockStyle.Fill, Padding = new Padding(4) };
        pnlValor.Controls.Add(new Label { Text = "Valor Adquisicion ($):", Font = new Font("Segoe UI", 9, FontStyle.Bold), Dock = DockStyle.Top, Height = 22, ForeColor = Color.FromArgb(60, 80, 100) });
        numValor = new NumericUpDown { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10), Maximum = 9999999, DecimalPlaces = 2, ThousandsSeparator = true };
        pnlValor.Controls.Add(numValor);
        layout.Controls.Add(pnlValor, 0, 3);

        var pnlEstado = new Panel { Dock = DockStyle.Fill, Padding = new Padding(4) };
        pnlEstado.Controls.Add(new Label { Text = "Estado:", Font = new Font("Segoe UI", 9, FontStyle.Bold), Dock = DockStyle.Top, Height = 22, ForeColor = Color.FromArgb(60, 80, 100) });
        cmbEstado = new ComboBox { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10), DropDownStyle = ComboBoxStyle.DropDownList };
        cmbEstado.Items.AddRange(new object[] { "Disponible", "Asignado", "En Reparacion", "Dado de Baja" });
        cmbEstado.SelectedIndex = 0;
        pnlEstado.Controls.Add(cmbEstado);
        layout.Controls.Add(pnlEstado, 1, 3);

        var pnlFecha = new Panel { Dock = DockStyle.Fill, Padding = new Padding(4) };
        pnlFecha.Controls.Add(new Label { Text = "Fecha Adquisicion:", Font = new Font("Segoe UI", 9, FontStyle.Bold), Dock = DockStyle.Top, Height = 22, ForeColor = Color.FromArgb(60, 80, 100) });
        dtpAdquisicion = new DateTimePicker { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10), Format = DateTimePickerFormat.Short };
        pnlFecha.Controls.Add(dtpAdquisicion);
        layout.Controls.Add(pnlFecha, 0, 4);

        txtDescripcion = AgregarCampo(layout, "Descripcion:", 1, 4);

        var pnlBts = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom, Height = 55,
            FlowDirection = FlowDirection.RightToLeft, Padding = new Padding(5)
        };
        btnEliminar = new Button
        {
            Text = "Eliminar", Width = 110, Height = 38,
            BackColor = Color.FromArgb(200, 60, 60), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Enabled = false
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
        var txt = new TextBox { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10), BorderStyle = BorderStyle.FixedSingle };
        pnl.Controls.Add(txt);
        layout.Controls.Add(pnl, col, row);
        return txt;
    }

    private void CargarDatos(string buscar = "")
    {
        var datos = string.IsNullOrEmpty(buscar) ? _repo.ObtenerTodos() : _repo.Buscar(buscar);
        dgv.Rows.Clear();
        foreach (var i in datos)
            dgv.Rows.Add(i.Id, i.Codigo, i.Nombre, i.Tipo, i.Marca, i.Estado);
    }

    private void Dgv_SelectionChanged(object? sender, EventArgs e)
    {
        if (dgv.SelectedRows.Count == 0) return;
        var id = (int)dgv.SelectedRows[0].Cells["Id"].Value;
        var obj = _repo.ObtenerPorId(id);
        if (obj == null) return;
        _idSeleccionado = obj.Id;
        txtCodigo.Text = obj.Codigo; txtNombre.Text = obj.Nombre;
        cmbTipo.SelectedItem = obj.Tipo; txtMarca.Text = obj.Marca;
        txtModelo.Text = obj.Modelo; txtNumeroSerie.Text = obj.NumeroSerie;
        cmbEstado.SelectedItem = obj.Estado; numValor.Value = obj.ValorAdquisicion;
        dtpAdquisicion.Value = obj.FechaAdquisicion; txtDescripcion.Text = obj.Descripcion;
        btnEliminar.Enabled = true;
    }

    private void BtnGuardar_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtNombre.Text))
        {
            MessageBox.Show("El nombre es requerido.", "Validacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        if (_idSeleccionado == 0)
        {
            _repo.Agregar(new Instrumento
            {
                Codigo = txtCodigo.Text.Trim(), Nombre = txtNombre.Text.Trim(),
                Tipo = cmbTipo.SelectedItem?.ToString() ?? "Otro", Marca = txtMarca.Text.Trim(),
                Modelo = txtModelo.Text.Trim(), NumeroSerie = txtNumeroSerie.Text.Trim(),
                Estado = cmbEstado.SelectedItem?.ToString() ?? "Disponible",
                ValorAdquisicion = numValor.Value, FechaAdquisicion = dtpAdquisicion.Value,
                Descripcion = txtDescripcion.Text.Trim()
            });
        }
        else
        {
            var obj = _repo.ObtenerPorId(_idSeleccionado)!;
            obj.Codigo = txtCodigo.Text.Trim(); obj.Nombre = txtNombre.Text.Trim();
            obj.Tipo = cmbTipo.SelectedItem?.ToString() ?? "Otro"; obj.Marca = txtMarca.Text.Trim();
            obj.Modelo = txtModelo.Text.Trim(); obj.NumeroSerie = txtNumeroSerie.Text.Trim();
            obj.Estado = cmbEstado.SelectedItem?.ToString() ?? "Disponible";
            obj.ValorAdquisicion = numValor.Value; obj.FechaAdquisicion = dtpAdquisicion.Value;
            obj.Descripcion = txtDescripcion.Text.Trim();
            _repo.Actualizar(obj);
        }
        MessageBox.Show("Guardado correctamente.", "Exito", MessageBoxButtons.OK, MessageBoxIcon.Information);
        LimpiarFormulario();
        CargarDatos();
    }

    private void BtnEliminar_Click(object? sender, EventArgs e)
    {
        if (_idSeleccionado == 0) return;
        if (MessageBox.Show("¿Eliminar este instrumento?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        {
            _repo.Eliminar(_idSeleccionado);
            LimpiarFormulario();
            CargarDatos();
        }
    }

    private void LimpiarFormulario()
    {
        _idSeleccionado = 0;
        txtCodigo.Text = txtNombre.Text = txtMarca.Text = txtModelo.Text =
        txtNumeroSerie.Text = txtDescripcion.Text = string.Empty;
        cmbTipo.SelectedIndex = 0; cmbEstado.SelectedIndex = 0;
        numValor.Value = 0; dtpAdquisicion.Value = DateTime.Now;
        btnEliminar.Enabled = false;
        dgv.ClearSelection();
    }
}
