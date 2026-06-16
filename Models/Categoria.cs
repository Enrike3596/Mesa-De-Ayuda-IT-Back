namespace Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Categoria  
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Area")]
        public int AreaId { get; set; }

        public  string Nombre { get; set; } = null!;
        public  string Descripcion { get; set; } = null!;
        public  Boolean Estado { get; set; } = true;

        public virtual  Area Area { get; set; } = null!;

        // Relación: CATEGORIA ||--o{ TICKET
        public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

        // Relación: CATEGORIA ||--o{ SUBCATEGORIA
        public virtual ICollection<Subcategoria> Subcategorias { get; set; } = new List<Subcategoria>();
    }
}