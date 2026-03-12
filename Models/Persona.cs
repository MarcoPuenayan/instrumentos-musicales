namespace RegistroInstrumentos.Models;

public class Persona
{
    public int Id { get; set; }
    public string Cedula { get; set; } = string.Empty;
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public string Departamento { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;
    public DateTime FechaRegistro { get; set; } = DateTime.Now;

    public string NombreCompleto => $"{Nombres} {Apellidos}";

    public ICollection<AsignacionInstrumento> AsignacionesInstrumento { get; set; } = new List<AsignacionInstrumento>();
    public ICollection<AsignacionRepuesto> AsignacionesRepuesto { get; set; } = new List<AsignacionRepuesto>();
}
