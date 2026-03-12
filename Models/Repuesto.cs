namespace RegistroInstrumentos.Models;

public class Repuesto
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public int StockActual { get; set; }
    public int StockMinimo { get; set; } = 1;
    public decimal Costo { get; set; }
    public int? InstrumentoId { get; set; }
    public Instrumento? Instrumento { get; set; }
    public bool Activo { get; set; } = true;
    public DateTime FechaRegistro { get; set; } = DateTime.Now;

    public ICollection<AsignacionRepuesto> Asignaciones { get; set; } = new List<AsignacionRepuesto>();
}
