using RegistroInstrumentos.Models;
using RegistroInstrumentos.Repositories;

namespace RegistroInstrumentos.Forms;

public class InstrumentoForm : Form
{
    private readonly InstrumentoRepository _repo;
    private DataGridView dgv = null!;
    private TextBox txtBuscar = null!;
    private int _idSeleccionado = 0;

    private TextBox txtCodigo = null!, txtNombre = null!, txtMarca = null!;
    private TextBox txtModelo = null!, txtNumeroSerie = null!, txtDescripcion = null!;
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
        Size = new Size(1100, 810);
        MinimumSize = new Size(900, 650);
        BackColor = Color.FromArgb(240, 244, 248);

        // SplitContainer: Panel1=form (top), Panel2=list (bottom)
        var split = new SplitContainer
        {
            Dock = DockStyle.Fill, Orientation = Orientation.Horizontal,
            SplitterDistance = 445, SplitterWidth = 4,
            Panel1MinSize = 200, Panel2MinSize = 150,
            BackColor = Color.FromArgb(200, 215, 230)
        };
        Controls.Add(split);
        var pnlDetalle = split.Panel1;
        pnlDetalle.Padding = new Padding(18, 12, 18, 10);

        var lblForm = new Label
        {
            Text = "Datos del Instrumento", Font = new Font("Segoe UI", 13, FontStyle.Bold),
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

        // Layout 2 columnas x 5 filas, cada fila 72px
        var layout = CrearLayout(2, 5, 72);

        txtCodigo = AgregarCampo(layout, "Codigo:", 0, 0);
        txtNombre = AgregarCampo(layout, "Nombre del Instrumento:", 1, 0);

        cmbTipo = AgregarCombo(layout, "Tipo:", 0, 1,
            "Cuerda", "Viento Madera", "Viento Metal", "Percusion", "Teclado", "Electronico", "Otro");
        txtMarca = AgregarCampo(layout, "Marca:", 1, 1);

        txtModelo = AgregarCampo(layout, "Modelo:", 0, 2);
        txtNumeroSerie = AgregarCampo(layout, "Numero de Serie:", 1, 2);

        numValor = AgregarNumerico(layout, "Valor Adquisicion ($):", 0, 3, 9999999, 2);
        cmbEstado = AgregarCombo(layout, "Estado:", 1, 3,
            "Disponible", "Asignado", "En Reparacion", "Dado de Baja");

        dtpAdquisicion = AgregarFecha(layout, "Fecha Adquisicion:", 0, 4);
        txtDescripcion = AgregarCampo(layout, "Descripcion / Notas:", 1, 4);

        pnlDetalle.Controls.Add(layout);
        pnlDetalle.Controls.Add(pnlBts);
        pnlDetalle.Controls.Add(lblForm);

        // ── Panel inferior (lista) ───────────────────────────────────────────
        var pnlLista = split.Panel2;
        pnlLista.Padding = new Padding(10);

        var lblTitulo = new Label
        {
            Text = "Inventario de Instrumentos Musicales",
            Font = new Font("Segoe UI", 13, FontStyle.Bold),
            Dock = DockStyle.Top, Height = 40, ForeColor = Color.FromArgb(30, 80, 120)
        };

        var pnlBuscar = new Panel { Dock = DockStyle.Top, Height = 42 };
        txtBuscar = new TextBox { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 11), PlaceholderText = "Buscar por nombre, codigo o marca..." };
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

        dgv = CrearDataGridView();
        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Id", Visible = false });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Codigo", HeaderText = "Codigo", FillWeight = 18 });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Nombre", HeaderText = "Nombre" });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Tipo", HeaderText = "Tipo", FillWeight = 20 });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Marca", HeaderText = "Marca", FillWeight = 22 });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Estado", HeaderText = "Estado", FillWeight = 22 });
        dgv.SelectionChanged += Dgv_SelectionChanged;

        pnlLista.Controls.Add(dgv);
        pnlLista.Controls.Add(pnlBuscar);
        pnlLista.Controls.Add(lblTitulo);
    }

    // ── Helpers UI ──────────────────────────────────────────────────────────

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
            CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal, GridColor = Color.FromArgb(225, 235, 245)
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

    private static TableLayoutPanel CrearLayout(int cols, int rows, int rowH)
    {
        var tlp = new TableLayoutPanel
        {
            Dock = DockStyle.Fill, ColumnCount = cols, RowCount = rows,
            Padding = new Padding(0, 6, 0, 0)
        };
        for (int c = 0; c < cols; c++)
            tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / cols));
        for (int r = 0; r < rows; r++)
            tlp.RowStyles.Add(new RowStyle(SizeType.Absolute, rowH));
        return tlp;
    }

    private static TextBox AgregarCampo(TableLayoutPanel layout, string etiq, int col, int row)
    {
        var flow = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown,
            WrapContents = false, Padding = new Padding(4, 4, 4, 2)
        };
        var lbl = new Label
        {
            Text = etiq, Font = new Font("Segoe UI", 9, FontStyle.Bold),
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
        layout.Controls.Add(flow, col, row);
        return txt;
    }

    private static ComboBox AgregarCombo(TableLayoutPanel layout, string etiq, int col, int row, params string[] items)
    {
        var flow = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown,
            WrapContents = false, Padding = new Padding(4, 4, 4, 2)
        };
        var lbl = new Label
        {
            Text = etiq, Font = new Font("Segoe UI", 9, FontStyle.Bold),
            ForeColor = Color.FromArgb(55, 75, 105), AutoSize = false, Height = 20, Width = 400
        };
        var cmb = new ComboBox
        {
            Font = new Font("Segoe UI", 10), DropDownStyle = ComboBoxStyle.DropDownList,
            Height = 28, Width = 400
        };
        cmb.Items.AddRange(items);
        cmb.SelectedIndex = 0;
        flow.Controls.Add(lbl);
        flow.Controls.Add(cmb);
        flow.SizeChanged += (_, _) =>
        {
            int w = flow.ClientSize.Width - 10;
            if (w > 0) { lbl.Width = w; cmb.Width = w; }
        };
        layout.Controls.Add(flow, col, row);
        return cmb;
    }

    private static NumericUpDown AgregarNumerico(TableLayoutPanel layout, string etiq, int col, int row, decimal max, int decimales)
    {
        var flow = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown,
            WrapContents = false, Padding = new Padding(4, 4, 4, 2)
        };
        var lbl = new Label
        {
            Text = etiq, Font = new Font("Segoe UI", 9, FontStyle.Bold),
            ForeColor = Color.FromArgb(55, 75, 105), AutoSize = false, Height = 20, Width = 400
        };
        var num = new NumericUpDown
        {
            Font = new Font("Segoe UI", 10), Maximum = max, DecimalPlaces = decimales,
            ThousandsSeparator = decimales > 0, Height = 28, Width = 400
        };
        flow.Controls.Add(lbl);
        flow.Controls.Add(num);
        flow.SizeChanged += (_, _) =>
        {
            int w = flow.ClientSize.Width - 10;
            if (w > 0) { lbl.Width = w; num.Width = w; }
        };
        layout.Controls.Add(flow, col, row);
        return num;
    }

    private static DateTimePicker AgregarFecha(TableLayoutPanel layout, string etiq, int col, int row)
    {
        var flow = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown,
            WrapContents = false, Padding = new Padding(4, 4, 4, 2)
        };
        var lbl = new Label
        {
            Text = etiq, Font = new Font("Segoe UI", 9, FontStyle.Bold),
            ForeColor = Color.FromArgb(55, 75, 105), AutoSize = false, Height = 20, Width = 400
        };
        var dtp = new DateTimePicker
        {
            Font = new Font("Segoe UI", 10), Format = DateTimePickerFormat.Short,
            Height = 28, Width = 400
        };
        flow.Controls.Add(lbl);
        flow.Controls.Add(dtp);
        flow.SizeChanged += (_, _) =>
        {
            int w = flow.ClientSize.Width - 10;
            if (w > 0) { lbl.Width = w; dtp.Width = w; }
        };
        layout.Controls.Add(flow, col, row);
        return dtp;
    }

    private static Button CrearBoton(string texto, Color color, bool bold = false)
    {
        var btn = new Button
        {
            Text = texto, Width = 110, Height = 38, BackColor = color, ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand,
            Font = bold ? new Font("Segoe UI", 10, FontStyle.Bold) : new Font("Segoe UI", 9)
        };
        btn.FlatAppearance.BorderSize = 0;
        return btn;
    }

    // ── Logica ──────────────────────────────────────────────────────────────

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
            MessageBox.Show("El nombre es requerido.", "Validacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); return;
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
        LimpiarFormulario(); CargarDatos();
    }

    private void BtnEliminar_Click(object? sender, EventArgs e)
    {
        if (_idSeleccionado == 0) return;
        if (MessageBox.Show("¿Eliminar este instrumento?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        {
            _repo.Eliminar(_idSeleccionado); LimpiarFormulario(); CargarDatos();
        }
    }

    private void LimpiarFormulario()
    {
        _idSeleccionado = 0;
        txtCodigo.Text = txtNombre.Text = txtMarca.Text = txtModelo.Text =
        txtNumeroSerie.Text = txtDescripcion.Text = string.Empty;
        cmbTipo.SelectedIndex = 0; cmbEstado.SelectedIndex = 0;
        numValor.Value = 0; dtpAdquisicion.Value = DateTime.Now;
        btnEliminar.Enabled = false; dgv.ClearSelection(); txtCodigo.Focus();
    }
}
