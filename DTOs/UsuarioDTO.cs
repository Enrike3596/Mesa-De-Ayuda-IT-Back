namespace DTOs
{
    using System.ComponentModel.DataAnnotations;
    using System.Text.Json.Serialization;

    // Para crear un usuario
    public class UsuarioCreateDTO
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(150, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 150 caracteres.")]
        public required string Nombre { get; set; }

        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "El correo no tiene un formato válido.")]
        [StringLength(254, MinimumLength = 5, ErrorMessage = "El correo debe tener entre 5 y 254 caracteres.")]
        public required string Correo { get; set; }

        [Required(ErrorMessage = "El teléfono es obligatorio.")]
        [StringLength(30, MinimumLength = 6, ErrorMessage = "El teléfono debe tener entre 6 y 30 caracteres.")]
        public required string Telefono { get; set; }

        [JsonPropertyName("contrasenaHash")]
        [StringLength(200, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
        public string? ContrasenaHash { get; set; }

        [JsonPropertyName("contrasena")]
        [StringLength(200, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
        public string? Contrasena { get; set; }

        // Opcional en creación (por defecto true/Activo)
        public Boolean? Estado{ get; set; } = true;

        [Range(1, int.MaxValue, ErrorMessage = "RolId debe ser mayor que 0.")]
        public int RolId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "AreaId debe ser mayor que 0.")]
        public int AreaId { get; set; }
    }

    // Para responder (nunca exponer la contraseña)
    public class UsuarioResponseDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public Boolean Estado { get; set; } = true;
        public int RolId { get; set; }
        public string Rol { get; set; } = string.Empty;
        public int AreaId { get; set; }
        public string Area { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
    }

    // Para actualizar
    public class UsuarioUpdateDTO
    {
        [StringLength(150, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 150 caracteres.")]
        public string? Nombre { get; set; }

        [StringLength(30, MinimumLength = 6, ErrorMessage = "El teléfono debe tener entre 6 y 30 caracteres.")]
        public string? Telefono { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "RolId debe ser mayor que 0.")]
        public int? RolId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "AreaId debe ser mayor que 0.")]
        public int? AreaId { get; set; }

        public Boolean? Estado { get; set; }
    }
}
