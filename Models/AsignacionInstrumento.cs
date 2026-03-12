namespace RegistroInstrumentos.Models;

public class AsignacionInstrumento
{
    public int Id { get; set; }
    public int InstrumentoId { get; set; }
    public Instrumento Instrumento { get; set; } = null!;
    public int PersonaId { get; set; }
    public Persona Persona { get; set; } = null!;
    public DateTime FechaAsignacion { get; set; } = DateTime.Now;
    public DateTime? FechaDevolucion { get; set; }
    public string Estado { get; set; } = "Activa"; // Activa | Devuelto
    public string Observaciones { get; set; } = string.Empty;
    public string UsuarioRegistro { get; set; } = string.Empty;
}
