namespace DTOs
{
    using System.ComponentModel.DataAnnotations;

    public class TicketCreateDTO
    {
        [Required(ErrorMessage = "El título es obligatorio.")]
        [StringLength(150, MinimumLength = 3, ErrorMessage = "El título debe tener entre 3 y 150 caracteres.")]
        public required string Titulo { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        [StringLength(2000, MinimumLength = 5, ErrorMessage = "La descripción debe tener entre 5 y 2000 caracteres.")]
        public required string Descripcion { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "UsuarioId debe ser mayor que 0.")]
        public required int UsuarioId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "CategoriaId debe ser mayor que 0.")]
        public required int CategoriaId { get; set; }

        public int? SubcategoriaId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "PrioridadId debe ser mayor que 0.")]
        public required int PrioridadId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "AreaId debe ser mayor que 0.")]
        public required int AreaId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "TipoTicketId debe ser mayor que 0.")]
        public required int TipoTicketId { get; set; }
    }

    public class CategoriaInfo
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
    }

    public class SubcategoriaInfo
    {
        public int Id { get; set; }
        public string NombreSubcategoria { get; set; } = string.Empty;
    }

    public class PrioridadInfo
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int Hora_sla { get; set; }
    }

    public class TipoTicketInfo
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
    }

    public class TicketResponseDTO
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public int UsuarioId { get; set; }
        public int CategoriaId { get; set; }
        public int? SubcategoriaId { get; set; }
        public int PrioridadId { get; set; }
        public int AreaId { get; set; }
        public int TipoTicketId { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaCierre { get; set; }

        // Confirmación de cierre
        public DateTime? FechaSolicitudCierre { get; set; }
        public DateTime? FechaConfirmacionCierre { get; set; }
        public string? MotivoRechazo { get; set; }
        public int? CerradoPorUsuarioId { get; set; }

        // SLA
        public DateTime? FechaLimiteSLA { get; set; }
        public string? EstadoSLA { get; set; }
        public bool SLAVencido { get; set; }
        public DateTime? FechaPausaSLA { get; set; }
        public int TiempoAcumuladoPausaMinutos { get; set; }
        public int HorasSLA { get; set; }

        // Navegación (para incluir en SignalR y API)
        public CategoriaInfo? Categoria { get; set; }
        public SubcategoriaInfo? Subcategoria { get; set; }
        public PrioridadInfo? Prioridad { get; set; }
        public TipoTicketInfo? TipoTicket { get; set; }
    }   

    public class TicketSlaResponseDTO
    {
        public int Id { get; set; }
        public DateTime? FechaLimiteSLA { get; set; }
        public string? EstadoSLA { get; set; }
        public bool SLAVencido { get; set; }
        public int MinutosRestantes { get; set; }
        public int TiempoVencidoMins { get; set; }
        public string? Prioridad { get; set; }
        public int HorasSLA { get; set; }
        public DateTime? FechaPausaSLA { get; set; }
        public int TiempoAcumuladoPausaMinutos { get; set; }
    }
    public class TicketUpdateDTO
    {
        [StringLength(150, MinimumLength = 3, ErrorMessage = "El título debe tener entre 3 y 150 caracteres.")]
        public string? Titulo { get; set; }

        [StringLength(2000, MinimumLength = 5, ErrorMessage = "La descripción debe tener entre 5 y 2000 caracteres.")]
        public string? Descripcion { get; set; }

        // Adaptable: tolera mayúsculas/espacios/guiones/guion_bajo
        [RegularExpression(@"(?i)^\s*(abierto|en[\s_-]*proceso|en[\s_-]*espera|programado|pendiente[\s_-]*confirmacion|reabierto|cerrado|resuelto)\s*$", ErrorMessage = "El estado debe ser 'Abierto', 'En Proceso', 'En Espera', 'Programado', 'Pendiente Confirmacion', 'Reabierto', 'Cerrado' o 'Resuelto'.")]
        public string? Estado { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "UsuarioId debe ser mayor que 0.")]
        public int? UsuarioId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "CategoriaId debe ser mayor que 0.")]
        public int? CategoriaId { get; set; }

        public int? SubcategoriaId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "PrioridadId debe ser mayor que 0.")]
        public int? PrioridadId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "AreaId debe ser mayor que 0.")]
        public int? AreaId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "TipoTicketId debe ser mayor que 0.")]
        public int? TipoTicketId { get; set; }
    }
}
