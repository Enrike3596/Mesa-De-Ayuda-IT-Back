namespace Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class TicketAsignado
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Ticket")]
        public int TicketId { get; set; }

        [ForeignKey("UsuarioAgente")]
        public int UsuarioAgenteId { get; set; }

        public DateTime FechaAsignacion { get; set; }

        public virtual  Ticket Ticket { get; set; } = null!;
        public virtual  Usuario UsuarioAgente { get; set; } = null!;
    }
}