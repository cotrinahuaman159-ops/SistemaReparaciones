namespace SistemaReparaciones.Models
{
    public class SolicitudAdminViewModel
    {
        public int Id { get; set; }

        public string ClienteId { get; set; } = string.Empty;

        public string NombreCliente { get; set; } = string.Empty;

        public string Electrodomestico { get; set; } = string.Empty;

        public string DescripcionFalla { get; set; } = string.Empty;

        public string Direccion { get; set; } = string.Empty;

        public string Telefono { get; set; } = string.Empty;

        public DateTime FechaSolicitada { get; set; }

        public DateTime FechaRegistro { get; set; }

        public string Estado { get; set; } = string.Empty;

        public string? TecnicoId { get; set; }

        public string? NombreTecnico { get; set; }

        // Información del diagnóstico
        public int? DiagnosticoId { get; set; }

        public string? DiagnosticoDescripcion { get; set; }

        public string? DiagnosticoObservaciones { get; set; }

        public DateTime? DiagnosticoFecha { get; set; }

        public int? ReparacionId { get; set; }

        public string? ReparacionTrabajoRealizado { get; set; }

        public string? ReparacionRepuestosUtilizados { get; set; }
        public decimal? ReparacionCostoServicio { get; set; }

        public string? ReparacionObservaciones { get; set; }

        public DateTime? ReparacionFecha { get; set; }


        // Información de la calificación
        public int? CalificacionId { get; set; }

        public int? CalificacionPuntuacion { get; set; }

        public string? CalificacionComentario { get; set; }

        public DateTime? CalificacionFecha { get; set; }
    }
}
