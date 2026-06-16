namespace Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Rol")]
        public int RolId { get; set; }

        [ForeignKey("Area")]
        public int AreaId { get; set; }

        public  string Nombre { get; set; } = null!;
        public  string Correo { get; set; } = null!;
        public  string Telefono { get; set; } = null!;
        public  string ContrasenaHash  { get; set; } = null!;
        public  Boolean Estado { get; set; } = true;
        public  DateTime FechaCreacion { get; set; } 
        public  DateTime FechaModificacion { get; set; } 

        public virtual Rol Rol { get; set; } = null!;
        public virtual Area Area { get; set; } = null!;

        // Relación: USUARIO ||--o{ TICKET (crea)
        public virtual ICollection<Ticket> TicketsCreados { get; set; } = new List<Ticket>();

        // Relación: USUARIO ||--o{ TICKET_ASIGNADO (resuelve)
        public virtual ICollection<TkAsignado> TkAsignados { get; set; } = new List<TkAsignado>();

        // Relación: USUARIO ||--o{ TICKET_COMENTARIO (escribe)
        public virtual ICollection<TkComentario> TkComentarios { get; set; } = new List<TkComentario>();

        // Relación: USUARIO ||--o{ TK_ANEXO (carga)
        public virtual ICollection<TkAnexo> TkAnexos { get; set; } = new List<TkAnexo>();

        // Relación: USUARIO ||--o{ TICKET (cierra)
        public virtual ICollection<Ticket> TicketsCerrados { get; set; } = new List<Ticket>();

        // Relación: USUARIO ||--o{ HISTORIAL_TICKET
        public virtual ICollection<HistorialTicket> HistorialTickets { get; set; } = new List<HistorialTicket>();
    }
}