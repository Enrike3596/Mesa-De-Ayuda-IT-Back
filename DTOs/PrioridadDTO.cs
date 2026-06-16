namespace DTOs
{   
    using System.ComponentModel.DataAnnotations;

    // Para crear una prioridad
    public class PrioridadCreateDTO
    {
        [Required(ErrorMessage = "El nombre de la prioridad es obligatorio.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre de la prioridad debe tener entre 2 y 100 caracteres.")]
        public required string NombrePrioridad { get; set; }

        [Required(ErrorMessage = "El tipo es obligatorio.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "El tipo debe tener entre 2 y 50 caracteres.")]
        public required string Tipo { get; set; }

        [Range(0, 100000, ErrorMessage = "HoraSla debe estar entre 0 y 100000.")]
        public int Hora_sla  { get; set; }

        // Adaptable: acepta variantes (espacios/mayúsculas) pero valida solo Activo/Inactivo
        public required Boolean Estado { get; set; }
    }

    // Para responder
    public class PrioridadResponseDTO
    {
        public int Id { get; set; }
        public string NombrePrioridad { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public int Hora_sla  { get; set; }
        public Boolean Estado { get; set; }
    }

    // Para actualizar
    public class PrioridadUpdateDTO
    {
        [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre de la prioridad debe tener entre 2 y 100 caracteres.")]
        public string? NombrePrioridad { get; set; }

        [StringLength(50, MinimumLength = 2, ErrorMessage = "El tipo debe tener entre 2 y 50 caracteres.")]
        public string? Tipo { get; set; }

        [Range(0, 100000, ErrorMessage = "HoraSla debe estar entre 0 y 100000.")]
        public int? Hora_sla  { get; set; }

        public Boolean? Estado { get; set; }
    }   
    
}