namespace DTOs
{
    public class NotificacionResponseDTO
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int TicketId { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;
        public bool Leida { get; set; }
        public DateTime FechaCreacion { get; set; }
    }

    public class NotificacionCountDTO
    {
        public int Cantidad { get; set; }
    }
}
