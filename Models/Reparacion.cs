using System.ComponentModel.DataAnnotations;

namespace SistemaReparaciones.Models
{
    public class Reparacion
    {
        public int Id { get; set; }

        // Solicitud a la que pertenece la reparación
        public int SolicitudServicioId { get; set; }

        public SolicitudServicio? SolicitudServicio { get; set; }

        // Técnico que realizó la reparación
        public string TecnicoId { get; set; } = string.Empty;

        public ApplicationUser? Tecnico { get; set; }

        [Required(ErrorMessage = "Ingrese el trabajo realizado.")]
        [Display(Name = "Trabajo realizado")]
        public string TrabajoRealizado { get; set; } = string.Empty;

        [Display(Name = "Repuestos utilizados")]
        public string? RepuestosUtilizados { get; set; }

        [Display(Name = "Observaciones")]
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
        [Required(ErrorMessage = "Ingrese el costo del servicio.")]
        [Range(0, 999999.99, ErrorMessage = "Ingrese un costo válido.")]
        [Display(Name = "Costo del servicio")]
        public string? Observaciones { get; set; }       
        public decimal CostoServicio { get; set; }
    }
}
