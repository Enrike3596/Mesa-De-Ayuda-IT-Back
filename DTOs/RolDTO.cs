namespace DTOs
{
    using System.ComponentModel.DataAnnotations;

    public class RolCreateDTO
    {
        [Required(ErrorMessage = "El nombre del rol es obligatorio.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre del rol debe tener entre 2 y 100 caracteres.")]
        public required string NombreRol { get; set; }

        [Required(ErrorMessage = "El tipo de rol es obligatorio.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "El tipo de rol debe tener entre 2 y 50 caracteres.")]
        public required string Tipo { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio.")]
        [RegularExpression(@"(?i)^\s*(activo|inactivo)\s*$", ErrorMessage = "El estado debe ser 'Activo' o 'Inactivo'.")]
        public required Boolean Estado { get; set; }
    }

    public class RolResponseDTO
    {
        public int Id { get; set; }
        public string NombreRol { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public Boolean Estado { get; set; } = true;
    }

    public class RolUpdateDTO
    {
        [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre del rol debe tener entre 2 y 100 caracteres.")]
        public string? NombreRol { get; set; }

        [StringLength(50, MinimumLength = 2, ErrorMessage = "El tipo de rol debe tener entre 2 y 50 caracteres.")]
        public string? Tipo { get; set; }

        [RegularExpression(@"(?i)^\s*(activo|inactivo)\s*$", ErrorMessage = "El estado debe ser 'Activo' o 'Inactivo'.")]
        public Boolean? Estado { get; set; }
    }
    
}