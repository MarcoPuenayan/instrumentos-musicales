using RegistroInstrumentos.Models;
using RegistroInstrumentos.Repositories;
using RegistroInstrumentos.Services;

namespace RegistroInstrumentos.Forms;

public class AsignacionInstrumentoForm : Form
{
    private readonly AsignacionInstrumentoRepository _repo;
    private readonly InstrumentoRepository _instrRepo;
    private readonly PersonaRepository _personaRepo;
    private DataGridView dgv = null!;
    private ComboBox cmbInstrumento = null!, cmbPersona = null!;
    private DateTimePicker dtpFecha = null!;
    private TextBox txtObservaciones = null!;
    private Button btnAsignar = null!, btnDevolver = null!;
    private int _idSeleccionado = 0;

    public AsignacionInstrumentoForm(AsignacionInstrumentoRepository repo,
        InstrumentoRepository instrRepo, PersonaRepository personaRepo)
    {
        _repo = repo; _instrRepo = instrRepo; _personaRepo = personaRepo;
        InicializarComponentes();
        CargarDatos();
    }

    private void InicializarComponentes()
    {
        Text = "Asignacion de Instrumentos a Personas";
        Size = new Size(1150, 680);
        BackColor = Color.FromArgb(240, 244, 248);

        var pnlLista = new Panel { Dock = DockStyle.Left, Width = 660, Padding = new Padding(10) };
        Controls.Add(pnlLista);
        pnlLista.Controls.Add(new Label
        {
            Text = "Historial de Asignaciones", Font = new Font("Segoe UI", 14, FontStyle.Bold),
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
        dgv.SelectionChanged += Dgv_SelectionChanged;
        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Id", Visible = false });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Instrumento", HeaderText = "Instrumento" });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Persona", HeaderText = "Asignado a" });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "FechaAsig", HeaderText = "Fecha Asignacion", FillWeight = 20 });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "FechaDev", HeaderText = "Fecha Devolucion", FillWeight = 20 });
        dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Estado", HeaderText = "Estado", FillWeight = 15 });
        pnlLista.Controls.Add(dgv);

        Controls.Add(new Panel { Dock = DockStyle.Left, Width = 4, BackColor = Color.FromArgb(200, 215, 230) });

        var pnlDetalle = new Panel { Dock = DockStyle.Fill, Padding = new Padding(15) };
        Controls.Add(pnlDetalle);
        pnlDetalle.Controls.Add(new Label
        {
            Text = "Nueva Asignacion", Font = new Font("Segoe UI", 13, FontStyle.Bold),
            Dock = DockStyle.Top, Height = 40, ForeColor = Color.FromArgb(30, 80, 120)
        });

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 5,
            Padding = new Padding(0, 5, 0, 0)
        };
        for (int i = 0; i < 5; i++) layout.RowStyles.Add(new RowStyle(SizeType.Percent, 20));

        var pnlInst = CrearPanel(layout, "Instrumento Disponible:", 0);
        cmbInstrumento = new ComboBox { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10), DropDownStyle = ComboBoxStyle.DropDownList };
        pnlInst.Controls.Add(cmbInstrumento);

        var pnlPer = CrearPanel(layout, "Asignar a Persona:", 1);
        cmbPersona = new ComboBox { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10), DropDownStyle = ComboBoxStyle.DropDownList };
        pnlPer.Controls.Add(cmbPersona);

        var pnlFecha = CrearPanel(layout, "Fecha de Asignacion:", 2);
        dtpFecha = new DateTimePicker { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10), Format = DateTimePickerFormat.Short };
        pnlFecha.Controls.Add(dtpFecha);

        var pnlObs = CrearPanel(layout, "Observaciones:", 3);
        txtObservaciones = new TextBox { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10), Multiline = true };
        pnlObs.Controls.Add(txtObservaciones);

        var pnlBts = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom, Height = 55,
            FlowDirection = FlowDirection.RightToLeft, Padding = new Padding(5)
        };
        btnDevolver = new Button
        {
            Text = "Registrar Devolucion", Width = 175, Height = 38,
            BackColor = Color.FromArgb(180, 100, 20), ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat, Enabled = false, Font = new Font("Segoe UI", 9, FontStyle.Bold)
        };
        btnDevolver.FlatAppearance.BorderSize = 0;
        btnDevolver.Click += BtnDevolver_Click;

        btnAsignar = new Button
        {
            Text = "Registrar Asignacion", Width = 175, Height = 38,
            BackColor = Color.FromArgb(30, 80, 120), ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9, FontStyle.Bold)
        };
        btnAsignar.FlatAppearance.BorderSize = 0;
        btnAsignar.Click += BtnAsignar_Click;

        pnlBts.Controls.AddRange(new Control[] { btnDevolver, btnAsignar });
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
        cmbInstrumento.DataSource = _instrRepo.ObtenerDisponibles();
        cmbInstrumento.DisplayMember = "Nombre";
        cmbInstrumento.ValueMember = "Id";

        cmbPersona.DataSource = _personaRepo.ObtenerTodos();
        cmbPersona.DisplayMember = "NombreCompleto";
        cmbPersona.ValueMember = "Id";
    }

    private void CargarDatos()
    {
        var datos = _repo.ObtenerTodos();
        dgv.Rows.Clear();
        foreach (var a in datos)
            dgv.Rows.Add(a.Id, a.Instrumento.Nombre,
                $"{a.Persona.Nombres} {a.Persona.Apellidos}",
                a.FechaAsignacion.ToString("dd/MM/yyyy"),
                a.FechaDevolucion?.ToString("dd/MM/yyyy") ?? "-",
                a.Estado);
    }

    private void Dgv_SelectionChanged(object? sender, EventArgs e)
    {
        if (dgv.SelectedRows.Count == 0) return;
        _idSeleccionado = (int)dgv.SelectedRows[0].Cells["Id"].Value;
        var a = _repo.ObtenerPorId(_idSeleccionado);
        btnDevolver.Enabled = a?.Estado == "Activa";
    }

    private void BtnAsignar_Click(object? sender, EventArgs e)
    {
        if (cmbInstrumento.SelectedItem == null || cmbPersona.SelectedItem == null)
        {
            MessageBox.Show("Seleccione instrumento y persona.", "Validacion", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        var instrId = (int)cmbInstrumento.SelectedValue!;
        var personaId = (int)cmbPersona.SelectedValue!;

        _repo.Agregar(new AsignacionInstrumento
        {
            InstrumentoId = instrId, PersonaId = personaId,
            FechaAsignacion = dtpFecha.Value, Estado = "Activa",
            Observaciones = txtObservaciones.Text.Trim(),
            UsuarioRegistro = AuthService.UsuarioActual?.NombreUsuario ?? "sistema"
        });

        var instr = _instrRepo.ObtenerPorId(instrId)!;
        instr.Estado = "Asignado";
        _instrRepo.Actualizar(instr);

        MessageBox.Show("Asignacion registrada correctamente.", "Exito", MessageBoxButtons.OK, MessageBoxIcon.Information);
        txtObservaciones.Text = string.Empty;
        CargarCombos();
        CargarDatos();
    }

    private void BtnDevolver_Click(object? sender, EventArgs e)
    {
        if (_idSeleccionado == 0) return;
        var a = _repo.ObtenerPorId(_idSeleccionado)!;
        a.Estado = "Devuelto";
        a.FechaDevolucion = DateTime.Now;
        _repo.Actualizar(a);

        var instr = _instrRepo.ObtenerPorId(a.InstrumentoId)!;
        instr.Estado = "Disponible";
        _instrRepo.Actualizar(instr);

        MessageBox.Show("Devolucion registrada.", "Exito", MessageBoxButtons.OK, MessageBoxIcon.Information);
        _idSeleccionado = 0;
        btnDevolver.Enabled = false;
        CargarCombos();
        CargarDatos();
    }
}
