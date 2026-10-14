using System.ComponentModel.DataAnnotations;

namespace SistemaReparaciones.Models
{
    public class Calificacion
    {
        public int Id { get; set; }

        // Solicitud que está siendo calificada
        public int SolicitudServicioId { get; set; }

        public SolicitudServicio? SolicitudServicio { get; set; }

        // Cliente que realiza la calificación
        public string ClienteId { get; set; } = string.Empty;

        public ApplicationUser? Cliente { get; set; }

        // Técnico que realizó el servicio
        public string TecnicoId { get; set; } = string.Empty;

        public ApplicationUser? Tecnico { get; set; }

        [Required(ErrorMessage = "Seleccione una puntuación.")]
        [Range(1, 5, ErrorMessage = "La puntuación debe estar entre 1 y 5.")]
        [Display(Name = "Puntuación")]
        public int Puntuacion { get; set; }

        [Display(Name = "Comentario")]
        public string? Comentario { get; set; }

        public DateTime FechaRegistro { get; set; } = DateTime.Now;
    }
}