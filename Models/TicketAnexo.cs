namespace Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class TicketAnexo
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Ticket")]
        public int TicketId { get; set; }

        [ForeignKey("Usuario")]
        public int UsuarioId { get; set; }

        public string NombreArchivo { get; set; } = null!;
        public string TipoArchivo { get; set; } = null!;
        public long TamanoArchivo { get; set; }
        public string UrlArchivo { get; set; } = null!;
        public DateTime FechaCarga { get; set; }
        public string Estado { get; set; } = null!;

        public virtual Ticket Ticket { get; set; } = null!;
        public virtual Usuario Usuario { get; set; } = null!;
    }
}