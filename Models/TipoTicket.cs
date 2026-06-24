namespace Models
{
    using System.ComponentModel.DataAnnotations;

    public class TipoTicket
    {
        [Key]
        public int Id { get; set; }

        public string Nombre { get; set; } = null!;

        public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
        public virtual ICollection<Categoria> Categorias { get; set; } = new List<Categoria>();
    }
}
