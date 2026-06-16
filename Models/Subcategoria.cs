namespace Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Subcategoria
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Categoria")]
        public int CategoriaId { get; set; }

        public  string NombreSubcategoria { get; set; } = null!;
        public string Descripcion { get; set; } = null!;
        public  Boolean Estado { get; set; } = true;

        public virtual Categoria Categoria { get; set; } = null!;
    }
}