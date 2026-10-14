using System.ComponentModel.DataAnnotations;

namespace SistemaReparaciones.Models
{
    public class SolicitudServicio
    {
        public int Id { get; set; }

        // Cliente que realiza la solicitud
       
        public string ClienteId { get; set; } = string.Empty;

        public ApplicationUser? Cliente { get; set; }

        [Required(ErrorMessage = "Ingrese el tipo de electrodoméstico.")]
        [Display(Name = "Electrodoméstico")]
        public int ElectrodomesticoId { get; set; }

        public Electrodomestico? Electrodomestico { get; set; }

        [Required(ErrorMessage = "Describa la falla.")]
        [Display(Name = "Descripción de la falla")]
        public string DescripcionFalla { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ingrese la dirección.")]
        public string Direccion { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ingrese el teléfono.")]
        public string Telefono { get; set; } = string.Empty;

        [Required(ErrorMessage = "Seleccione una fecha.")]
        [Display(Name = "Fecha solicitada")]
        [DataType(DataType.Date)]
        public  DateTime FechaSolicitada { get; set; }

        // Pendiente, Asignada, En reparación, Finalizada
        public string Estado { get; set; } = "Pendiente";

        // Técnico asignado posteriormente
        public string? TecnicoId { get; set; }

        public ApplicationUser? Tecnico { get; set; }

        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        // Diagnóstico realizado por el técnico
        public Diagnostico? Diagnostico { get; set; }

        // Reparación realizada por el técnico
        public Reparacion? Reparacion { get; set; }
        // Calificación realizada por el cliente
        public Calificacion? Calificacion { get; set; }
    }
}