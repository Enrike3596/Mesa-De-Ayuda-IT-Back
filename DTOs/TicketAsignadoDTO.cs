namespace DTOs
{
    using System.ComponentModel.DataAnnotations;

    // Para asignar un ticket a un usuario
    public class TicketAsignadoCreateDTO
    {
        [Range(1, int.MaxValue, ErrorMessage = "TicketId debe ser mayor que 0.")]
        public int TicketId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "UsuarioId debe ser mayor que 0.")]
        public int UsuarioId { get; set; }
    }

    public class TicketAsignadoResponseDTO
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public int UsuarioId { get; set; }
    
    }
    public class TicketAsignadoUpdateDTO
    {
        [Range(1, int.MaxValue, ErrorMessage = "TicketId debe ser mayor que 0.")]
        public int? TicketId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "UsuarioId debe ser mayor que 0.")]
        public int? UsuarioId { get; set; }
    }
}


