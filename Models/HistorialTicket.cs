namespace Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class HistorialTicket
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Ticket")]
        public int TicketId { get; set; }

        [ForeignKey("Usuario")]
        public int UsuarioId { get; set; }

        public string Descripcion { get; set; } = null!;
        public DateTime FechaAccion { get; set; }

        public virtual Ticket Ticket { get; set; } = null!;
        public virtual Usuario Usuario { get; set; } = null!;
    }
}
