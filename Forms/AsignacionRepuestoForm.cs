using RegistroInstrumentos.Models;
using RegistroInstrumentos.Repositories;
using RegistroInstrumentos.Services;

namespace RegistroInstrumentos.Forms;

public class AsignacionRepuestoForm : Form
{
    private readonly AsignacionRepuestoRepository _repo;
    private readonly RepuestoRepository _repuestoRepo;
    private readonly PersonaRepository _personaRepo;
    private DataGridView dgv = null!;
    private ComboBox cmbRepuesto = null!, cmbPersona = null!;
    private NumericUpDown numCantidad = null!;
    private DateTimePicker dtpFecha = null!;
    private TextBox txtMotivo = null!, txtObservaciones = null!;
    private Button btnRegistrar = null!;

    public AsignacionRepuestoForm(AsignacionRepuestoRepository repo,
        RepuestoRepository repuestoRepo, PersonaRepository personaRepo)
    {
        _repo = repo; _repuestoRepo = repuestoRepo; _personaRepo = personaRepo;
        InicializarComponentes();
        CargarCombos();
        CargarDatos();
    }

    private void InicializarComponentes()
    {
        Text = "Asignacion de Repuestos a Personas";
        Size = new Size(1200, 700);
        MinimumSize = new Size(900, 500);
        BackColor = Color.FromArgb(240, 244, 248);

        // SplitContainer: Panel1=form (top), Panel2=list (bottom)
        var split = new SplitContainer
        {
            Dock = DockStyle.Fill, Orientation = Orientation.Horizontal,
            SplitterDistance = 400, SplitterWidth = 4,
            Panel1MinSize = 200, Panel2MinSize = 150,
            BackColor = Color.FromArgb(200, 215, 230)
        };
        Controls.Add(split);
        var pnlDetalle = split.Panel1;
        pnlDetalle.Padding = new Padding(18, 12, 18, 10);

        var lblForm = new Label { Text = "Registrar Entrega de Repuesto", Font = new Font("Segoe UI", 13, FontStyle.Bold), Dock = DockStyle.Top, Height = 38, ForeColor = Color.FromArgb(30, 80, 120) };

        var pnlBts = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 52, FlowDirection = FlowDirection.RightToLeft, Padding = new Padding(0, 7, 0, 0) };
        btnRegistrar = Btn("Registrar Entrega", Color.FromArgb(30, 80, 120), bold: true, width: 155);
        btnRegistrar.Click += BtnRegistrar_Click;
        pnlBts.Controls.Add(btnRegistrar);

        var layout = CrearLayout(1, 6, 55);
        cmbRepuesto = Combo(layout, "Repuesto a Entregar:", 0, 0);
        cmbPersona = Combo(layout, "Persona que Recibe:", 0, 1);
        numCantidad = Numerico(layout, "Cantidad:", 0, 2, 9999, 0); numCantidad.Minimum = 1; numCantidad.Value = 1;
        dtpFecha = Fecha(layout, "Fecha de Entrega:", 0, 3);
        txtMotivo = Campo(layout, "Motivo / Razon:", 0, 4);
        txtObservaciones = Campo(layout, "Observaciones:", 0, 5);

        pnlDetalle.Controls.Add(layout);
        pnlDetalle.Controls.Add(pnlBts);
        pnlDetalle.Controls.Add(lblForm);

        // ── Panel inferior: historial ────────────────────────────────────────
        var pnlLista = split.Panel2;
        pnlLista.Padding = new Padding(10);

        var lblTitLista = new Label { Text = "Historial de Entrega de Repuestos", Font = new Font("Segoe UI", 13, FontStyle.Bold), Dock = DockStyle.Top, Height = 40, ForeColor = Color.FromArgb(30, 80, 120) };

        dgv = CrearDgv();
        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Id", Visible = false });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Repuesto", HeaderText = "Repuesto" });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Persona", HeaderText = "Entregado a" });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Cantidad", HeaderText = "Cant.", FillWeight = 10 });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Fecha", HeaderText = "Fecha", FillWeight = 18 });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Motivo", HeaderText = "Motivo", FillWeight = 22 });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Usuario", HeaderText = "Registrado por", FillWeight = 18 });

        pnlLista.Controls.Add(dgv);
        pnlLista.Controls.Add(lblTitLista);
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

    private static ComboBox Combo(TableLayoutPanel layout, string etiq, int col, int row)
    {
        var flow = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, WrapContents = false, Padding = new Padding(4, 4, 4, 2) };
        var lbl = new Label { Text = etiq, Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = Color.FromArgb(55, 75, 105), AutoSize = false, Height = 20, Width = 400 };
        var cmb = new ComboBox { Font = new Font("Segoe UI", 10), DropDownStyle = ComboBoxStyle.DropDownList, Height = 28, Width = 400 };
        flow.Controls.Add(lbl); flow.Controls.Add(cmb);
        flow.SizeChanged += (_, _) => { int w = flow.ClientSize.Width - 10; if (w > 0) { lbl.Width = w; cmb.Width = w; } };
        layout.Controls.Add(flow, col, row); return cmb;
    }

    private static NumericUpDown Numerico(TableLayoutPanel layout, string etiq, int col, int row, decimal max, int dec)
    {
        var flow = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, WrapContents = false, Padding = new Padding(4, 4, 4, 2) };
        var lbl = new Label { Text = etiq, Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = Color.FromArgb(55, 75, 105), AutoSize = false, Height = 20, Width = 400 };
        var num = new NumericUpDown { Font = new Font("Segoe UI", 10), Maximum = max, DecimalPlaces = dec, Height = 28, Width = 400 };
        flow.Controls.Add(lbl); flow.Controls.Add(num);
        flow.SizeChanged += (_, _) => { int w = flow.ClientSize.Width - 10; if (w > 0) { lbl.Width = w; num.Width = w; } };
        layout.Controls.Add(flow, col, row); return num;
    }

    private static DateTimePicker Fecha(TableLayoutPanel layout, string etiq, int col, int row)
    {
        var flow = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, WrapContents = false, Padding = new Padding(4, 4, 4, 2) };
        var lbl = new Label { Text = etiq, Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = Color.FromArgb(55, 75, 105), AutoSize = false, Height = 20, Width = 400 };
        var dtp = new DateTimePicker { Font = new Font("Segoe UI", 10), Format = DateTimePickerFormat.Short, Height = 28, Width = 400 };
        flow.Controls.Add(lbl); flow.Controls.Add(dtp);
        flow.SizeChanged += (_, _) => { int w = flow.ClientSize.Width - 10; if (w > 0) { lbl.Width = w; dtp.Width = w; } };
        layout.Controls.Add(flow, col, row); return dtp;
    }

    private static Button Btn(string texto, Color color, bool bold = false, int width = 120)
    {
        var b = new Button { Text = texto, Width = width, Height = 38, BackColor = color, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand, Font = bold ? new Font("Segoe UI", 10, FontStyle.Bold) : new Font("Segoe UI", 9) };
        b.FlatAppearance.BorderSize = 0; return b;
    }

    private void CargarCombos()
    {
        cmbRepuesto.DataSource = _repuestoRepo.ObtenerTodos();
        cmbRepuesto.DisplayMember = "Nombre"; cmbRepuesto.ValueMember = "Id";
        cmbPersona.DataSource = _personaRepo.ObtenerTodos();
        cmbPersona.DisplayMember = "NombreCompleto"; cmbPersona.ValueMember = "Id";
    }

    private void CargarDatos()
    {
        var datos = _repo.ObtenerTodos(); dgv.Rows.Clear();
        foreach (var a in datos)
            dgv.Rows.Add(a.Id, a.Repuesto.Nombre, $"{a.Persona.Nombres} {a.Persona.Apellidos}",
                a.Cantidad, a.FechaEntrega.ToString("dd/MM/yyyy"), a.Motivo, a.UsuarioRegistro);
    }

    private void BtnRegistrar_Click(object? sender, EventArgs e)
    {
        if (cmbRepuesto.SelectedItem == null || cmbPersona.SelectedItem == null)
        { MessageBox.Show("Seleccione repuesto y persona.", "Validacion", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
        var repuestoId = (int)cmbRepuesto.SelectedValue!;
        var repuesto = _repuestoRepo.ObtenerPorId(repuestoId)!;
        if (repuesto.StockActual < (int)numCantidad.Value)
        { MessageBox.Show($"Stock insuficiente. Disponible: {repuesto.StockActual}", "Stock Insuficiente", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
        _repo.Agregar(new AsignacionRepuesto
        {
            RepuestoId = repuestoId, PersonaId = (int)cmbPersona.SelectedValue!,
            Cantidad = (int)numCantidad.Value, FechaEntrega = dtpFecha.Value,
            Motivo = txtMotivo.Text.Trim(), Observaciones = txtObservaciones.Text.Trim(),
            UsuarioRegistro = AuthService.UsuarioActual?.NombreUsuario ?? "sistema"
        });
        repuesto.StockActual -= (int)numCantidad.Value;
        _repuestoRepo.Actualizar(repuesto);
        MessageBox.Show("Entrega registrada correctamente.", "Exito", MessageBoxButtons.OK, MessageBoxIcon.Information);
        txtMotivo.Text = txtObservaciones.Text = string.Empty; numCantidad.Value = 1;
        CargarCombos(); CargarDatos();
    }
}
