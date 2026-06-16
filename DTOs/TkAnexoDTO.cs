namespace DTOs
{
    using System.ComponentModel.DataAnnotations;

    // Para crear un anexo
    public class TkAnexoCreateDTO
    {
        [Required(ErrorMessage = "El nombre del archivo es obligatorio.")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "El nombre del archivo debe tener entre 1 y 200 caracteres.")]
        public required string NombreArchivo { get; set; }

        [Required(ErrorMessage = "El tipo de archivo es obligatorio.")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "El tipo de archivo debe tener entre 1 y 100 caracteres.")]
        public required string TipoArchivo { get; set; }

        [Range(0, long.MaxValue, ErrorMessage = "TamanoArchivo debe ser mayor o igual a 0.")]
        public long TamanoArchivo { get; set; }

        [Required(ErrorMessage = "La URL del archivo es obligatoria.")]
        [StringLength(2048, MinimumLength = 1, ErrorMessage = "La URL del archivo debe tener entre 1 y 2048 caracteres.")]
        public required string UrlArchivo { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "UsuarioId debe ser mayor que 0.")]
        public int UsuarioId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "TicketId debe ser mayor que 0.")]
        public int TicketId { get; set; }

        // Adaptable: opcional en creación (por defecto Activo), pero valida si se envía
        [RegularExpression(@"(?i)^\s*(activo|inactivo)\s*$", ErrorMessage = "El estado debe ser 'Activo' o 'Inactivo'.")]
        public string? Estado { get; set; }
    }

    // Para responder
    public class TkAnexoResponseDTO
    {
        public int Id { get; set; }
        public string NombreArchivo { get; set; } = string.Empty;
        public int TicketId { get; set; }
        public int UsuarioId { get; set; }

        public string UrlArchivo { get; set; } = string.Empty;

        public string TipoArchivo { get; set; } = string.Empty;

        public long TamanoArchivo { get; set; }

        public DateTime FechaCarga { get; set; }

        public string Estado { get; set; } = string.Empty;
        
    }

    // para actulizar un anexo
    public class TkAnexoUpdateDTO
    {
        [StringLength(200, MinimumLength = 1, ErrorMessage = "El nombre del archivo debe tener entre 1 y 200 caracteres.")]
        public string? NombreArchivo { get; set; }

        [StringLength(100, MinimumLength = 1, ErrorMessage = "El tipo de archivo debe tener entre 1 y 100 caracteres.")]
        public string? TipoArchivo { get; set; }

        [Range(0, long.MaxValue, ErrorMessage = "TamanoArchivo debe ser mayor o igual a 0.")]
        public long? TamanoArchivo { get; set; }

        [StringLength(2048, MinimumLength = 1, ErrorMessage = "La URL del archivo debe tener entre 1 y 2048 caracteres.")]
        public string? UrlArchivo { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "UsuarioId debe ser mayor que 0.")]
        public int? UsuarioId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "TicketId debe ser mayor que 0.")]
        public int? TicketId { get; set; }

        [RegularExpression(@"(?i)^\s*(activo|inactivo)\s*$", ErrorMessage = "El estado debe ser 'Activo' o 'Inactivo'.")]
        public string? Estado { get; set; }
    }
}