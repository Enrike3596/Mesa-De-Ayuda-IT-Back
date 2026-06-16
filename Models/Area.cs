namespace Models
{
    using System.ComponentModel.DataAnnotations;

    public class Area
    {
        [Key]
        public int Id { get; set; }
        public  string NombreArea { get; set; } = null!;
        public  Boolean Estado { get; set; } = true;

        // Relación: AREA ||--o{ USUARIO
        public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();

        // Relación: AREA ||--o{ CATEGORIA
        public virtual ICollection<Categoria> Categorias { get; set; } = new List<Categoria>();

        // Relación evidenciada en el modelo actual: AREA ||--o{ TICKET
        public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}