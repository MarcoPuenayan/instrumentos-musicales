using RegistroInstrumentos.Models;
using RegistroInstrumentos.Repositories;

namespace RegistroInstrumentos.Forms;

public class RepuestoForm : Form
{
    private readonly RepuestoRepository _repo;
    private readonly InstrumentoRepository _instrRepo;
    private DataGridView dgv = null!;
    private TextBox txtBuscar = null!, txtCodigo = null!, txtNombre = null!;
    private TextBox txtCategoria = null!, txtDescripcion = null!;
    private NumericUpDown numStock = null!, numStockMin = null!, numCosto = null!;
    private ComboBox cmbInstrumento = null!;
    private Button btnGuardar = null!, btnEliminar = null!;
    private int _idSeleccionado = 0;

    public RepuestoForm(RepuestoRepository repo, InstrumentoRepository instrRepo)
    {
        _repo = repo; _instrRepo = instrRepo;
        InicializarComponentes();
        CargarDatos();
    }

    private void InicializarComponentes()
    {
        Text = "Registro de Repuestos para Instrumentos";
        Size = new Size(1100, 800);
        MinimumSize = new Size(900, 640);
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
        pnlDetalle.Padding = new Padding(18, 12, 18, 10);

        var lblForm = new Label { Text = "Datos del Repuesto", Font = new Font("Segoe UI", 13, FontStyle.Bold), Dock = DockStyle.Top, Height = 38, ForeColor = Color.FromArgb(30, 80, 120) };
        var pnlBts = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 52, FlowDirection = FlowDirection.RightToLeft, Padding = new Padding(0, 7, 0, 0) };
        btnEliminar = Btn("Eliminar", Color.FromArgb(200, 60, 60)); btnEliminar.Enabled = false; btnEliminar.Click += BtnEliminar_Click;
        btnGuardar = Btn("Guardar", Color.FromArgb(30, 80, 120), true); btnGuardar.Click += BtnGuardar_Click;
        var btnLimpiar = Btn("Limpiar", Color.FromArgb(100, 120, 140)); btnLimpiar.Click += (s, e) => LimpiarFormulario();
        pnlBts.Controls.AddRange(new Control[] { btnEliminar, btnGuardar, btnLimpiar });

        var layout = CrearLayout(2, 4, 72);
        txtCodigo = Campo(layout, "Codigo:", 0, 0);
        txtNombre = Campo(layout, "Nombre del Repuesto:", 1, 0);
        txtCategoria = Campo(layout, "Categoria:", 0, 1);

        var flowInst = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, WrapContents = false, Padding = new Padding(4, 4, 4, 2) };
        var lblInst = new Label { Text = "Instrumento Asociado (opcional):", Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = Color.FromArgb(55, 75, 105), AutoSize = false, Height = 20, Width = 400 };
        cmbInstrumento = new ComboBox { Font = new Font("Segoe UI", 10), DropDownStyle = ComboBoxStyle.DropDownList, Height = 28, Width = 400 };
        flowInst.Controls.Add(lblInst);
        flowInst.Controls.Add(cmbInstrumento);
        flowInst.SizeChanged += (_, _) => { int w = flowInst.ClientSize.Width - 10; if (w > 0) { lblInst.Width = w; cmbInstrumento.Width = w; } };
        layout.Controls.Add(flowInst, 1, 1);

        numStock = Numerico(layout, "Stock Actual:", 0, 2, 99999, 0);
        numStockMin = Numerico(layout, "Stock Minimo:", 1, 2, 99999, 0);
        numCosto = Numerico(layout, "Costo Unitario ($):", 0, 3, 99999, 2);
        txtDescripcion = Campo(layout, "Descripcion:", 1, 3);
        numStockMin.Value = 1;

        pnlDetalle.Controls.Add(layout);
        pnlDetalle.Controls.Add(pnlBts);
        pnlDetalle.Controls.Add(lblForm);

        // ── Panel inferior (lista) ───────────────────────────────────────────
        var pnlLista = split.Panel2;
        pnlLista.Padding = new Padding(10);

        var lblTitulo = new Label { Text = "Repuestos para Instrumentos", Font = new Font("Segoe UI", 13, FontStyle.Bold), Dock = DockStyle.Top, Height = 40, ForeColor = Color.FromArgb(30, 80, 120) };
        var pnlBuscar = new Panel { Dock = DockStyle.Top, Height = 42 };
        txtBuscar = new TextBox { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 11), PlaceholderText = "Buscar repuesto por nombre o codigo..." };
        txtBuscar.TextChanged += (s, e) => CargarDatos(txtBuscar.Text);
        var btnNuevo = new Button { Text = "Nuevo", Dock = DockStyle.Right, Width = 90, BackColor = Color.FromArgb(30, 120, 80), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
        btnNuevo.FlatAppearance.BorderSize = 0;
        btnNuevo.Click += (s, e) => LimpiarFormulario();
        pnlBuscar.Controls.Add(txtBuscar);
        pnlBuscar.Controls.Add(btnNuevo);

        dgv = CrearDgv();
        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Id", Visible = false });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Codigo", HeaderText = "Codigo", FillWeight = 15 });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Nombre", HeaderText = "Nombre" });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Categoria", HeaderText = "Categoria", FillWeight = 22 });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Stock", HeaderText = "Stock", FillWeight = 12 });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Instrumento", HeaderText = "Instrumento", FillWeight = 28 });
        dgv.SelectionChanged += Dgv_SelectionChanged;

        pnlLista.Controls.Add(dgv);
        pnlLista.Controls.Add(pnlBuscar);
        pnlLista.Controls.Add(lblTitulo);

        CargarComboInstrumentos();
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

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
        d.DefaultCellStyle.Padding = new Padding(4, 0, 0, 0);
        d.RowTemplate.Height = 30;
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

    private static NumericUpDown Numerico(TableLayoutPanel layout, string etiq, int col, int row, decimal max, int dec)
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
            Font = new Font("Segoe UI", 10), Maximum = max, DecimalPlaces = dec,
            ThousandsSeparator = dec > 0, Height = 28, Width = 400
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

    private static Button Btn(string texto, Color color, bool bold = false)
    {
        var b = new Button { Text = texto, Width = 110, Height = 38, BackColor = color, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand, Font = bold ? new Font("Segoe UI", 10, FontStyle.Bold) : new Font("Segoe UI", 9) };
        b.FlatAppearance.BorderSize = 0; return b;
    }

    // ── Logica ───────────────────────────────────────────────────────────────

    private void CargarComboInstrumentos()
    {
        cmbInstrumento.Items.Clear();
        cmbInstrumento.Items.Add(new ComboItem(null, "(Ninguno)"));
        foreach (var i in _instrRepo.ObtenerTodos())
            cmbInstrumento.Items.Add(new ComboItem(i.Id, i.Nombre));
        cmbInstrumento.DisplayMember = "Nombre"; cmbInstrumento.ValueMember = "Valor";
        cmbInstrumento.SelectedIndex = 0;
    }

    private void CargarDatos(string buscar = "")
    {
        var datos = string.IsNullOrEmpty(buscar) ? _repo.ObtenerTodos() : _repo.Buscar(buscar);
        dgv.Rows.Clear();
        foreach (var r in datos)
            dgv.Rows.Add(r.Id, r.Codigo, r.Nombre, r.Categoria, r.StockActual, r.Instrumento?.Nombre ?? "-");
    }

    private void Dgv_SelectionChanged(object? sender, EventArgs e)
    {
        if (dgv.SelectedRows.Count == 0) return;
        var id = (int)dgv.SelectedRows[0].Cells["Id"].Value;
        var obj = _repo.ObtenerPorId(id); if (obj == null) return;
        _idSeleccionado = obj.Id;
        txtCodigo.Text = obj.Codigo; txtNombre.Text = obj.Nombre;
        txtCategoria.Text = obj.Categoria; txtDescripcion.Text = obj.Descripcion;
        numStock.Value = obj.StockActual; numStockMin.Value = obj.StockMinimo; numCosto.Value = obj.Costo;
        for (int i = 0; i < cmbInstrumento.Items.Count; i++)
            if (cmbInstrumento.Items[i] is ComboItem ci && ci.Valor == obj.InstrumentoId) { cmbInstrumento.SelectedIndex = i; break; }
        btnEliminar.Enabled = true;
    }

    private void BtnGuardar_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtNombre.Text)) { MessageBox.Show("El nombre es requerido.", "Validacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
        int? instrId = (cmbInstrumento.SelectedItem as ComboItem)?.Valor;
        if (_idSeleccionado == 0)
        {
            _repo.Agregar(new Repuesto { Codigo = txtCodigo.Text.Trim(), Nombre = txtNombre.Text.Trim(), Categoria = txtCategoria.Text.Trim(), Descripcion = txtDescripcion.Text.Trim(), StockActual = (int)numStock.Value, StockMinimo = (int)numStockMin.Value, Costo = numCosto.Value, InstrumentoId = instrId });
        }
        else
        {
            var obj = _repo.ObtenerPorId(_idSeleccionado)!;
            obj.Codigo = txtCodigo.Text.Trim(); obj.Nombre = txtNombre.Text.Trim(); obj.Categoria = txtCategoria.Text.Trim(); obj.Descripcion = txtDescripcion.Text.Trim();
            obj.StockActual = (int)numStock.Value; obj.StockMinimo = (int)numStockMin.Value; obj.Costo = numCosto.Value; obj.InstrumentoId = instrId;
            _repo.Actualizar(obj);
        }
        MessageBox.Show("Guardado correctamente.", "Exito", MessageBoxButtons.OK, MessageBoxIcon.Information);
        LimpiarFormulario(); CargarDatos();
    }

    private void BtnEliminar_Click(object? sender, EventArgs e)
    {
        if (_idSeleccionado == 0) return;
        if (MessageBox.Show("¿Eliminar este repuesto?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        { _repo.Eliminar(_idSeleccionado); LimpiarFormulario(); CargarDatos(); }
    }

    private void LimpiarFormulario()
    {
        _idSeleccionado = 0;
        txtCodigo.Text = txtNombre.Text = txtCategoria.Text = txtDescripcion.Text = string.Empty;
        numStock.Value = numCosto.Value = 0; numStockMin.Value = 1;
        cmbInstrumento.SelectedIndex = 0; btnEliminar.Enabled = false;
        dgv.ClearSelection(); txtCodigo.Focus();
    }

    private record ComboItem(int? Valor, string Nombre) { public override string ToString() => Nombre; }
}
