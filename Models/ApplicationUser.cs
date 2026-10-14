using Microsoft.AspNetCore.Identity;

namespace SistemaReparaciones.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Nombres { get; set; } = string.Empty;

        public string Apellidos { get; set; } = string.Empty;

        // Especialidad del técnico
        public int? EspecialidadId { get; set; }

        public Electrodomestico? Especialidad { get; set; }

        public bool Disponible { get; set; } = true;
    }
}