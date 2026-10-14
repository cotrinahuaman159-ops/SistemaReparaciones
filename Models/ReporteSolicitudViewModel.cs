namespace SistemaReparaciones.Models
{
    public class ReporteSolicitudViewModel
    {
        public int Id { get; set; }

        public string Cliente { get; set; } = string.Empty;

        public string Electrodomestico { get; set; } = string.Empty;

        public string? Tecnico { get; set; }

        public string Estado { get; set; } = string.Empty;

        public DateTime FechaSolicitada { get; set; }

        public DateTime FechaRegistro { get; set; }
    }
}
