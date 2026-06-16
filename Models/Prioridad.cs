namespace Models
{
    using System.ComponentModel.DataAnnotations;

    public class Prioridad
    {
        [Key]
        public int Id { get; set; }
        public  string Nombre { get; set; } = null!;
        public  string Tipo { get; set; } = null!;
        public int Hora_sla { get; set; }
        public  Boolean Estado { get; set; } = true;

        // Relación: PRIORIDAD ||--o{ TICKET
        public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}