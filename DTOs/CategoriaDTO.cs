namespace DTOs
{
    using System.ComponentModel.DataAnnotations;

    // Para crear una categoría
    public class CategoriaCreateDTO
    {
        [Required(ErrorMessage = "El nombre de la categoría es obligatorio.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre de la categoría debe tener entre 2 y 100 caracteres.")]
        public required string NombreCategoria { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        [StringLength(500, MinimumLength = 2, ErrorMessage = "La descripción debe tener entre 2 y 500 caracteres.")]
        public required string Descripcion { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "AreaId debe ser mayor que 0.")]
        public int AreaId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "TipoTicketId debe ser mayor que 0.")]
        public int TipoTicketId { get; set; }

        public required Boolean Estado { get; set; } = true;
    }

    // Para responder

    public class CategoriaResponseDTO
    {
        public int Id { get; set; }
        public string NombreCategoria { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public int AreaId { get; set; }
        public int TipoTicketId { get; set; }
        public Boolean Estado { get; set; } = true;
    }

    // Para actualizar
    public class CategoriaUpdateDTO
    {
        [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre de la categoría debe tener entre 2 y 100 caracteres.")]
        public string? NombreCategoria { get; set; }

        [StringLength(500, MinimumLength = 2, ErrorMessage = "La descripción debe tener entre 2 y 500 caracteres.")]
        public string? Descripcion { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "AreaId debe ser mayor que 0.")]
        public int? AreaId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "TipoTicketId debe ser mayor que 0.")]
        public int? TipoTicketId { get; set; }

        public Boolean? Estado { get; set; }
    }

}