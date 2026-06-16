namespace DTOs
{
    using System.ComponentModel.DataAnnotations;

    // Para asignar un ticket a un usuario
    public class TkAsignadoCreateDTO
    {
        [Range(1, int.MaxValue, ErrorMessage = "TicketId debe ser mayor que 0.")]
        public int TicketId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "UsuarioId debe ser mayor que 0.")]
        public int UsuarioId { get; set; }
    }

    public class TkAsignadoResponseDTO
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public int UsuarioId { get; set; }
    
    }
    public class TkAsignadoUpdateDTO
    {
        [Range(1, int.MaxValue, ErrorMessage = "TicketId debe ser mayor que 0.")]
        public int? TicketId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "UsuarioId debe ser mayor que 0.")]
        public int? UsuarioId { get; set; }
    }
}


