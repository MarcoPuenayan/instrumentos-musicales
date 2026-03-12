namespace RegistroInstrumentos.Models;

public class Instrumento
{
    public int Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public string Marca { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
    public string NumeroSerie { get; set; } = string.Empty;
    public string Estado { get; set; } = "Disponible"; // Disponible | Asignado | En Reparacion | Dado de Baja
    public string Descripcion { get; set; } = string.Empty;
    public decimal ValorAdquisicion { get; set; }
    public DateTime FechaAdquisicion { get; set; } = DateTime.Now;
    public bool Activo { get; set; } = true;

    public ICollection<AsignacionInstrumento> Asignaciones { get; set; } = new List<AsignacionInstrumento>();
    public ICollection<Repuesto> Repuestos { get; set; } = new List<Repuesto>();
}
