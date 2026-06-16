namespace Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Ticket
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("UsuarioCreador")]
        public int UsuarioCreadorId { get; set; }

        [ForeignKey("Categoria")]
        public int CategoriaId { get; set; }

        [ForeignKey("Subcategoria")]
        public int? SubcategoriaId { get; set; }

        [ForeignKey("Prioridad")]
        public int PrioridadId { get; set; }
        [ForeignKey("Area")]
        public int AreaId { get; set; }

        [ForeignKey("TicketTipo")]
        public int TipoTicketId { get; set; }

        public  string Titulo { get; set; } = null!;
        public  string Descripcion { get; set; } = null!;
        public  string Estado { get; set; } = null!;
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaCierre { get; set; }

        // Confirmación de cierre
        public DateTime? FechaSolicitudCierre { get; set; }
        public DateTime? FechaConfirmacionCierre { get; set; }
        public string? MotivoRechazo { get; set; }

        [ForeignKey("CerradoPor")]
        public int? CerradoPorUsuarioId { get; set; }

        // SLA
        public DateTime? FechaLimiteSLA { get; set; } // null si la prioridad no define SLA
        public DateTime? FechaPausaSLA { get; set; }
        public int TiempoAcumuladoPausaMinutos { get; set; } = 0;
        public bool SLAVencido { get; set; } = false;
        public string? EstadoSLA { get; set; } // "En Tiempo", "Vencido", "Pausado", etc.

        public virtual  Usuario UsuarioCreador { get; set; } = null!;
        public virtual  Categoria Categoria { get; set; } = null!;
        public virtual  Subcategoria? Subcategoria { get; set; }
        public virtual  Prioridad Prioridad { get; set; } = null!;  
        public virtual  Area Area { get; set; } = null!;
        public virtual TipoTicket TicketTipo { get; set; } = null!;
        public virtual Usuario? CerradoPor { get; set; }

        // Relación: TICKET ||--o{ TICKET_ASIGNADO
        public virtual ICollection<TkAsignado> TkAsignados { get; set; } = new List<TkAsignado>();

        // Relación: TICKET ||--o{ TICKET_COMENTARIO
        public virtual ICollection<TkComentario> TkComentarios { get; set; } = new List<TkComentario>();

        // Relación: TICKET ||--o{ TK_ANEXO
        public virtual ICollection<TkAnexo> TkAnexos { get; set; } = new List<TkAnexo>();

        // Relación: TICKET ||--o{ HISTORIAL_TICKET
        public virtual ICollection<HistorialTicket> HistorialTickets { get; set; } = new List<HistorialTicket>();
    }
}
