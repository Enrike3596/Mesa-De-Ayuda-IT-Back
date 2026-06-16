namespace DTOs
{
    using System.ComponentModel.DataAnnotations;

    public class SolicitudCierreDTO
    {
        [Required]
        public string ResumenSolucion { get; set; } = null!;
    }

    public class ConfirmacionCierreDTO
    {
        [Required]
        public bool Aceptado { get; set; }

        public string? MotivoRechazo { get; set; }
    }
}
