namespace RegistroInstrumentos.Models;

public class AsignacionRepuesto
{
    public int Id { get; set; }
    public int RepuestoId { get; set; }
    public Repuesto Repuesto { get; set; } = null!;
    public int PersonaId { get; set; }
    public Persona Persona { get; set; } = null!;
    public int Cantidad { get; set; } = 1;
    public DateTime FechaEntrega { get; set; } = DateTime.Now;
    public string Motivo { get; set; } = string.Empty;
    public string Observaciones { get; set; } = string.Empty;
    public string UsuarioRegistro { get; set; } = string.Empty;
}
