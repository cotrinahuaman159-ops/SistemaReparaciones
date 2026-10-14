using System.ComponentModel.DataAnnotations;

namespace SistemaReparaciones.Models
{
    public class Diagnostico
    {
        public int Id { get; set; }

        // Solicitud a la que pertenece el diagnóstico
        public int SolicitudServicioId { get; set; }

        public SolicitudServicio? SolicitudServicio { get; set; }

        // Técnico que realizó el diagnóstico
        public string TecnicoId { get; set; } = string.Empty;

        public ApplicationUser? Tecnico { get; set; }

        [Required(ErrorMessage = "Ingrese el diagnóstico.")]
        [Display(Name = "Diagnóstico")]
        public string Descripcion { get; set; } = string.Empty;

        [Display(Name = "Observaciones")]
        public string? Observaciones { get; set; }

        public DateTime FechaRegistro { get; set; } = DateTime.Now;
    }
}
