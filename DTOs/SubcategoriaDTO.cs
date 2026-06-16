namespace DTOs
{
    using System.ComponentModel.DataAnnotations;

    // Para crear una subcategoría
    public class SubcategoriaCreateDTO
    {
        [Required(ErrorMessage = "El nombre de la subcategoría es obligatorio.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre de la subcategoría debe tener entre 2 y 100 caracteres.")]
        public required string NombreSubcategoria { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        [StringLength(500, MinimumLength = 2, ErrorMessage = "La descripción debe tener entre 2 y 500 caracteres.")]
        public required string Descripcion { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "CategoriaId debe ser mayor que 0.")]
        public int CategoriaId { get; set; }
        public required Boolean Estado { get; set; }
    }

    // Para responder
    public class SubcategoriaResponseDTO
    {
        public int Id { get; set; }
        public string NombreSubcategoria { get; set; } = string.Empty;
        public int CategoriaId { get; set; }
        public string Descripcion { get; set; } = string.Empty;
    
        public Boolean Estado { get; set; } = true;
    }

    // Para actualizar
    public class SubcategoriaUpdateDTO
    {   
        [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre de la subcategoría debe tener entre 2 y 100 caracteres.")]
        public string? NombreSubcategoria { get; set; }

        [StringLength(500, MinimumLength = 2, ErrorMessage = "La descripción debe tener entre 2 y 500 caracteres.")]
        public string? Descripcion { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "CategoriaId debe ser mayor que 0.")]
        public int? CategoriaId { get; set; }
        public Boolean? Estado { get; set; }
    }
}

    
