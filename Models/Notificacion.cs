namespace Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Notificacion
    {
        [Key]
        public int Id { get; set; }

        public int UsuarioId { get; set; }
        public int TicketId { get; set; }
        public string Tipo { get; set; } = null!;
        public string Mensaje { get; set; } = null!;
        public bool Leida { get; set; }
        public DateTime FechaCreacion { get; set; }

        [ForeignKey("UsuarioId")]
        public virtual Usuario Usuario { get; set; } = null!;

        [ForeignKey("TicketId")]
        public virtual Ticket Ticket { get; set; } = null!;
    }
}
