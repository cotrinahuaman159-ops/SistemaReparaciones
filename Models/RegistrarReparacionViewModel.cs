using System.ComponentModel.DataAnnotations;

namespace SistemaReparaciones.Models
{
    public class RegistrarReparacionViewModel
    {
        public int SolicitudServicioId { get; set; }

        [Required(ErrorMessage = "Ingrese el trabajo realizado.")]
        [Display(Name = "Trabajo realizado")]
        public string TrabajoRealizado { get; set; } = string.Empty;

        [Display(Name = "Repuestos utilizados")]
        public string? RepuestosUtilizados { get; set; }


        [Required(ErrorMessage = "Ingrese el costo del servicio.")]
        [Range(0, 999999.99,
            ErrorMessage = "Ingrese un costo válido.")]
        [Display(Name = "Costo del servicio")]
        public decimal CostoServicio { get; set; }

        [Display(Name = "Observaciones")]
        public string? Observaciones { get; set; }
      
    }
}













