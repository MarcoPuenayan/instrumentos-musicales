using RegistroInstrumentos.Models;
using RegistroInstrumentos.Repositories;

namespace RegistroInstrumentos.Forms;

public class RepuestoForm : Form
{
    private readonly RepuestoRepository _repo;
    private readonly InstrumentoRepository _instrRepo;
    private DataGridView dgv = null!;
    private TextBox txtBuscar = null!, txtCodigo = null!, txtNombre = null!;
    private TextBox txtDescripcion = null!, txtCategoria = null!;
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
        Size = new Size(1150, 680);
        BackColor = Color.FromArgb(240, 244, 248);

        var pnlLista = new Panel { Dock = DockStyle.Left, Width = 540, Padding = new Padding(10) };
        Controls.Add(pnlLista);
        pnlLista.Controls.Add(new Label
        {
            Text = "Repuestos para Instrumentos", Font = new Font("Segoe UI", 14, FontStyle.Bold),
            Dock = DockStyle.Top, Height = 40, ForeColor = Color.FromArgb(30, 80, 120)
        });

        var pnlBuscar = new Panel { Dock = DockStyle.Top, Height = 40 };
        txtBuscar = new TextBox { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 11), PlaceholderText = "Buscar repuesto..." };
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
        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Id", Visible = false });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Codigo", HeaderText = "Codigo", FillWeight = 18 });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Nombre", HeaderText = "Nombre" });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Categoria", HeaderText = "Categoria", FillWeight = 20 });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Stock", HeaderText = "Stock", FillWeight = 12 });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Instrumento", HeaderText = "Instrumento" });
        pnlLista.Controls.Add(dgv);

        Controls.Add(new Panel { Dock = DockStyle.Left, Width = 4, BackColor = Color.FromArgb(200, 215, 230) });

        var pnlDetalle = new Panel { Dock = DockStyle.Fill, Padding = new Padding(15) };
        Controls.Add(pnlDetalle);
        pnlDetalle.Controls.Add(new Label
        {
            Text = "Datos del Repuesto", Font = new Font("Segoe UI", 13, FontStyle.Bold),
            Dock = DockStyle.Top, Height = 40, ForeColor = Color.FromArgb(30, 80, 120)
        });

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 5,
            Padding = new Padding(0, 5, 0, 0)
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        for (int i = 0; i < 5; i++) layout.RowStyles.Add(new RowStyle(SizeType.Percent, 20));

        txtCodigo = AgregarCampo(layout, "Codigo:", 0, 0);
        txtNombre = AgregarCampo(layout, "Nombre:", 1, 0);
        txtCategoria = AgregarCampo(layout, "Categoria:", 0, 1);

        var pnlInst = new Panel { Dock = DockStyle.Fill, Padding = new Padding(4) };
        pnlInst.Controls.Add(new Label { Text = "Instrumento Asociado (opcional):", Font = new Font("Segoe UI", 9, FontStyle.Bold), Dock = DockStyle.Top, Height = 22, ForeColor = Color.FromArgb(60, 80, 100) });
        cmbInstrumento = new ComboBox { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10), DropDownStyle = ComboBoxStyle.DropDownList };
        pnlInst.Controls.Add(cmbInstrumento);
        layout.Controls.Add(pnlInst, 1, 1);

        var pnlStock = new Panel { Dock = DockStyle.Fill, Padding = new Padding(4) };
        pnlStock.Controls.Add(new Label { Text = "Stock Actual:", Font = new Font("Segoe UI", 9, FontStyle.Bold), Dock = DockStyle.Top, Height = 22, ForeColor = Color.FromArgb(60, 80, 100) });
        numStock = new NumericUpDown { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10), Maximum = 99999 };
        pnlStock.Controls.Add(numStock);
        layout.Controls.Add(pnlStock, 0, 2);

        var pnlStockMin = new Panel { Dock = DockStyle.Fill, Padding = new Padding(4) };
        pnlStockMin.Controls.Add(new Label { Text = "Stock Minimo:", Font = new Font("Segoe UI", 9, FontStyle.Bold), Dock = DockStyle.Top, Height = 22, ForeColor = Color.FromArgb(60, 80, 100) });
        numStockMin = new NumericUpDown { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10), Maximum = 99999, Value = 1 };
        pnlStockMin.Controls.Add(numStockMin);
        layout.Controls.Add(pnlStockMin, 1, 2);

        var pnlCosto = new Panel { Dock = DockStyle.Fill, Padding = new Padding(4) };
        pnlCosto.Controls.Add(new Label { Text = "Costo Unitario ($):", Font = new Font("Segoe UI", 9, FontStyle.Bold), Dock = DockStyle.Top, Height = 22, ForeColor = Color.FromArgb(60, 80, 100) });
        numCosto = new NumericUpDown { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10), Maximum = 99999, DecimalPlaces = 2, ThousandsSeparator = true };
        pnlCosto.Controls.Add(numCosto);
        layout.Controls.Add(pnlCosto, 0, 3);

        txtDescripcion = AgregarCampo(layout, "Descripcion:", 1, 3);

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

        CargarComboInstrumentos();
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

    private void CargarComboInstrumentos()
    {
        var instrumentos = _instrRepo.ObtenerTodos();
        cmbInstrumento.Items.Clear();
        cmbInstrumento.Items.Add("(Ninguno)");
        foreach (var i in instrumentos)
            cmbInstrumento.Items.Add(new ComboInstrumentoItem(i.Id, i.Nombre));
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
        var obj = _repo.ObtenerPorId(id);
        if (obj == null) return;
        _idSeleccionado = obj.Id;
        txtCodigo.Text = obj.Codigo; txtNombre.Text = obj.Nombre;
        txtCategoria.Text = obj.Categoria; txtDescripcion.Text = obj.Descripcion;
        numStock.Value = obj.StockActual; numStockMin.Value = obj.StockMinimo; numCosto.Value = obj.Costo;

        cmbInstrumento.SelectedIndex = 0;
        if (obj.InstrumentoId.HasValue)
        {
            for (int i = 1; i < cmbInstrumento.Items.Count; i++)
            {
                if (cmbInstrumento.Items[i] is ComboInstrumentoItem item && item.Id == obj.InstrumentoId.Value)
                {
                    cmbInstrumento.SelectedIndex = i; break;
                }
            }
        }
        btnEliminar.Enabled = true;
    }

    private void BtnGuardar_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtNombre.Text))
        {
            MessageBox.Show("El nombre es requerido.", "Validacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        int? instrId = cmbInstrumento.SelectedItem is ComboInstrumentoItem sel ? sel.Id : null;

        if (_idSeleccionado == 0)
        {
            _repo.Agregar(new Repuesto
            {
                Codigo = txtCodigo.Text.Trim(), Nombre = txtNombre.Text.Trim(),
                Categoria = txtCategoria.Text.Trim(), Descripcion = txtDescripcion.Text.Trim(),
                StockActual = (int)numStock.Value, StockMinimo = (int)numStockMin.Value,
                Costo = numCosto.Value, InstrumentoId = instrId
            });
        }
        else
        {
            var obj = _repo.ObtenerPorId(_idSeleccionado)!;
            obj.Codigo = txtCodigo.Text.Trim(); obj.Nombre = txtNombre.Text.Trim();
            obj.Categoria = txtCategoria.Text.Trim(); obj.Descripcion = txtDescripcion.Text.Trim();
            obj.StockActual = (int)numStock.Value; obj.StockMinimo = (int)numStockMin.Value;
            obj.Costo = numCosto.Value; obj.InstrumentoId = instrId;
            _repo.Actualizar(obj);
        }
        MessageBox.Show("Guardado correctamente.", "Exito", MessageBoxButtons.OK, MessageBoxIcon.Information);
        LimpiarFormulario();
        CargarDatos();
    }

    private void BtnEliminar_Click(object? sender, EventArgs e)
    {
        if (_idSeleccionado == 0) return;
        if (MessageBox.Show("¿Eliminar este repuesto?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        {
            _repo.Eliminar(_idSeleccionado);
            LimpiarFormulario();
            CargarDatos();
        }
    }

    private void LimpiarFormulario()
    {
        _idSeleccionado = 0;
        txtCodigo.Text = txtNombre.Text = txtCategoria.Text = txtDescripcion.Text = string.Empty;
        numStock.Value = numCosto.Value = 0; numStockMin.Value = 1;
        cmbInstrumento.SelectedIndex = 0;
        btnEliminar.Enabled = false;
        dgv.ClearSelection();
    }
}

internal class ComboInstrumentoItem
{
    public int Id { get; }
    public string Nombre { get; }
    public ComboInstrumentoItem(int id, string nombre) { Id = id; Nombre = nombre; }
    public override string ToString() => Nombre;
}
