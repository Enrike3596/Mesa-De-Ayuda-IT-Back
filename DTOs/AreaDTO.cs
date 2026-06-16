namespace DTOs
{
    using System.ComponentModel.DataAnnotations;

    // Para crear un área
    public class AreaCreateDTO
    {
        [Required(ErrorMessage = "El nombre del área es obligatorio.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre del área debe tener entre 2 y 100 caracteres.")]
        public required string NombreArea { get; set; }

        public required Boolean Estado { get; set; } = true;
    }

    // Para responder
    public class AreaResponseDTO
    {
        public int Id { get; set; }
        public string NombreArea { get; set; } = string.Empty;
        public Boolean Estado { get; set; } = true;
    }

    // Para actualizar
    public class AreaUpdateDTO
    {
        [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre del área debe tener entre 2 y 100 caracteres.")]
        public string? NombreArea { get; set; }
        public Boolean? Estado { get; set; }
    }
}