namespace SistemaReparaciones.Models
{
    public class TecnicoViewModel
    {
        public string Id { get; set; } = string.Empty;

        public string Nombres { get; set; } = string.Empty;

        public string Apellidos { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public int? EspecialidadId { get; set; }

        public string? EspecialidadNombre { get; set; }

        public bool Disponible { get; set; }
    }
}
