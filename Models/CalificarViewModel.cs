using System.ComponentModel.DataAnnotations;

namespace SistemaReparaciones.Models
{
    public class CalificarViewModel
    {
        public int SolicitudServicioId { get; set; }

        public string Electrodomestico { get; set; } = string.Empty;

        public string? NombreTecnico { get; set; }

        [Required(ErrorMessage = "Seleccione una puntuación.")]
        [Range(1, 5, ErrorMessage = "La puntuación debe estar entre 1 y 5.")]
        [Display(Name = "Puntuación")]
        public int Puntuacion { get; set; }

        [Display(Name = "Comentario")]
        public string? Comentario { get; set; }
    }
}
