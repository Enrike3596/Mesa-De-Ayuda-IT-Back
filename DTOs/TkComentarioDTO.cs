namespace DTOs
{
    using System.ComponentModel.DataAnnotations;
    using System.Text.Json.Serialization;

    // Para crear un comentario en un ticket
    public class TkComentarioCreateDTO
    {
        [Required(ErrorMessage = "El comentario es obligatorio.")]
        [StringLength(2000, MinimumLength = 1, ErrorMessage = "El comentario debe tener entre 1 y 2000 caracteres.")]
        public required string Comentario { get; set; }

        // Alias para compatibilidad con clientes existentes
        [JsonPropertyName("contenido")]
        public string? Contenido
        {
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                    Comentario = value;
            }
        }

        [JsonPropertyName("descripcion")]
        public string? Descripcion
        {
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                    Comentario = value;
            }
        }

        [Range(1, int.MaxValue, ErrorMessage = "UsuarioId debe ser mayor que 0.")]
        public int UsuarioId { get; set; }

        [JsonPropertyName("usuario_id")]
        public int? UsuarioIdSnake
        {
            set
            {
                if (value.HasValue) UsuarioId = value.Value;
            }
        }

        [Range(1, int.MaxValue, ErrorMessage = "TicketId debe ser mayor que 0.")]
        public int TicketId { get; set; }

        [JsonPropertyName("ticket_id")]
        public int? TicketIdSnake
        {
            set
            {
                if (value.HasValue) TicketId = value.Value;
            }
        }

        [JsonPropertyName("es_interno")]
        public bool EsInterno { get; set; }
    }
    // Para responder
    public class TkComentarioResponseDTO
    {
        public int Id { get; set; }
        public string Comentario { get; set; } = string.Empty;
        public int UsuarioId { get; set; }
        public int TicketId { get; set; }
        public bool EsInterno { get; set; }
    }

        // Para actualizar un comentario
    public class TkComentarioUpdateDTO
    {
        [StringLength(2000, MinimumLength = 1, ErrorMessage = "El comentario debe tener entre 1 y 2000 caracteres.")]
        public string? Comentario { get; set; }

        [JsonPropertyName("contenido")]
        public string? Contenido
        {
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                    Comentario = value;
            }
        }

        [JsonPropertyName("descripcion")]
        public string? Descripcion
        {
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                    Comentario = value;
            }
        }

        [Range(1, int.MaxValue, ErrorMessage = "UsuarioId debe ser mayor que 0.")]
        public int? UsuarioId { get; set; }

        [JsonPropertyName("usuario_id")]
        public int? UsuarioIdSnake
        {
            set
            {
                if (value.HasValue) UsuarioId = value.Value;
            }
        }

        [Range(1, int.MaxValue, ErrorMessage = "TicketId debe ser mayor que 0.")]
        public int? TicketId { get; set; }

        [JsonPropertyName("ticket_id")]
        public int? TicketIdSnake
        {
            set
            {
                if (value.HasValue) TicketId = value.Value;
            }
        }

        [JsonPropertyName("es_interno")]
        public bool? EsInterno { get; set; }
    }
}
