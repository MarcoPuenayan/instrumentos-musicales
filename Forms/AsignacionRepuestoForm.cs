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
        CargarDatos();
    }

    private void InicializarComponentes()
    {
        Text = "Asignacion y Entrega de Repuestos";
        Size = new Size(1150, 680);
        BackColor = Color.FromArgb(240, 244, 248);

        var pnlLista = new Panel { Dock = DockStyle.Left, Width = 660, Padding = new Padding(10) };
        Controls.Add(pnlLista);
        pnlLista.Controls.Add(new Label
        {
            Text = "Historial de Entrega de Repuestos", Font = new Font("Segoe UI", 14, FontStyle.Bold),
            Dock = DockStyle.Top, Height = 40, ForeColor = Color.FromArgb(30, 80, 120)
        });

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
        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Id", Visible = false });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Repuesto", HeaderText = "Repuesto" });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Persona", HeaderText = "Entregado a" });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Cantidad", HeaderText = "Cant.", FillWeight = 10 });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Fecha", HeaderText = "Fecha Entrega", FillWeight = 18 });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Motivo", HeaderText = "Motivo" });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Usuario", HeaderText = "Reg. por", FillWeight = 18 });
        pnlLista.Controls.Add(dgv);

        Controls.Add(new Panel { Dock = DockStyle.Left, Width = 4, BackColor = Color.FromArgb(200, 215, 230) });

        var pnlDetalle = new Panel { Dock = DockStyle.Fill, Padding = new Padding(15) };
        Controls.Add(pnlDetalle);
        pnlDetalle.Controls.Add(new Label
        {
            Text = "Registrar Entrega de Repuesto", Font = new Font("Segoe UI", 13, FontStyle.Bold),
            Dock = DockStyle.Top, Height = 40, ForeColor = Color.FromArgb(30, 80, 120)
        });

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 6,
            Padding = new Padding(0, 5, 0, 0)
        };
        for (int i = 0; i < 6; i++) layout.RowStyles.Add(new RowStyle(SizeType.Percent, 16.6f));

        var pnlRep = CrearPanel(layout, "Repuesto:", 0);
        cmbRepuesto = new ComboBox { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10), DropDownStyle = ComboBoxStyle.DropDownList };
        pnlRep.Controls.Add(cmbRepuesto);

        var pnlPer = CrearPanel(layout, "Persona que Recibe el Repuesto:", 1);
        cmbPersona = new ComboBox { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10), DropDownStyle = ComboBoxStyle.DropDownList };
        pnlPer.Controls.Add(cmbPersona);

        var pnlCant = CrearPanel(layout, "Cantidad a Entregar:", 2);
        numCantidad = new NumericUpDown { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10), Minimum = 1, Maximum = 9999, Value = 1 };
        pnlCant.Controls.Add(numCantidad);

        var pnlFecha = CrearPanel(layout, "Fecha de Entrega:", 3);
        dtpFecha = new DateTimePicker { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10), Format = DateTimePickerFormat.Short };
        pnlFecha.Controls.Add(dtpFecha);

        var pnlMotivo = CrearPanel(layout, "Motivo de Entrega:", 4);
        txtMotivo = new TextBox { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10), BorderStyle = BorderStyle.FixedSingle };
        pnlMotivo.Controls.Add(txtMotivo);

        var pnlObs = CrearPanel(layout, "Observaciones adicionales:", 5);
        txtObservaciones = new TextBox { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10), Multiline = true, BorderStyle = BorderStyle.FixedSingle };
        pnlObs.Controls.Add(txtObservaciones);

        var pnlBts = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom, Height = 55,
            FlowDirection = FlowDirection.RightToLeft, Padding = new Padding(5)
        };
        btnRegistrar = new Button
        {
            Text = "Registrar Entrega", Width = 175, Height = 38,
            BackColor = Color.FromArgb(30, 80, 120), ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10, FontStyle.Bold)
        };
        btnRegistrar.FlatAppearance.BorderSize = 0;
        btnRegistrar.Click += BtnRegistrar_Click;
        pnlBts.Controls.Add(btnRegistrar);
        pnlDetalle.Controls.Add(pnlBts);
        pnlDetalle.Controls.Add(layout);

        CargarCombos();
    }

    private Panel CrearPanel(TableLayoutPanel layout, string etiqueta, int row)
    {
        var pnl = new Panel { Dock = DockStyle.Fill, Padding = new Padding(4) };
        pnl.Controls.Add(new Label
        {
            Text = etiqueta, Font = new Font("Segoe UI", 9, FontStyle.Bold),
            Dock = DockStyle.Top, Height = 22, ForeColor = Color.FromArgb(60, 80, 100)
        });
        layout.Controls.Add(pnl, 0, row);
        return pnl;
    }

    private void CargarCombos()
    {
        cmbRepuesto.DataSource = _repuestoRepo.ObtenerTodos();
        cmbRepuesto.DisplayMember = "Nombre";
        cmbRepuesto.ValueMember = "Id";

        cmbPersona.DataSource = _personaRepo.ObtenerTodos();
        cmbPersona.DisplayMember = "NombreCompleto";
        cmbPersona.ValueMember = "Id";
    }

    private void CargarDatos()
    {
        var datos = _repo.ObtenerTodos();
        dgv.Rows.Clear();
        foreach (var a in datos)
            dgv.Rows.Add(a.Id, a.Repuesto.Nombre,
                $"{a.Persona.Nombres} {a.Persona.Apellidos}",
                a.Cantidad, a.FechaEntrega.ToString("dd/MM/yyyy"),
                a.Motivo, a.UsuarioRegistro);
    }

    private void BtnRegistrar_Click(object? sender, EventArgs e)
    {
        if (cmbRepuesto.SelectedItem == null || cmbPersona.SelectedItem == null)
        {
            MessageBox.Show("Seleccione repuesto y persona.", "Validacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        var repuestoId = (int)cmbRepuesto.SelectedValue!;
        var repuesto = _repuestoRepo.ObtenerPorId(repuestoId)!;

        if (repuesto.StockActual < (int)numCantidad.Value)
        {
            MessageBox.Show($"Stock insuficiente. Stock disponible: {repuesto.StockActual}",
                "Sin Stock", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        _repo.Agregar(new AsignacionRepuesto
        {
            RepuestoId = repuestoId,
            PersonaId = (int)cmbPersona.SelectedValue!,
            Cantidad = (int)numCantidad.Value,
            FechaEntrega = dtpFecha.Value,
            Motivo = txtMotivo.Text.Trim(),
            Observaciones = txtObservaciones.Text.Trim(),
            UsuarioRegistro = AuthService.UsuarioActual?.NombreUsuario ?? "sistema"
        });

        repuesto.StockActual -= (int)numCantidad.Value;
        _repuestoRepo.Actualizar(repuesto);

        MessageBox.Show("Entrega de repuesto registrada correctamente.", "Exito", MessageBoxButtons.OK, MessageBoxIcon.Information);
        txtMotivo.Text = txtObservaciones.Text = string.Empty;
        numCantidad.Value = 1;
        CargarCombos();
        CargarDatos();
    }
}
