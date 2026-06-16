namespace Models
{
    using System.ComponentModel.DataAnnotations;

    public class Rol
    {
        [Key]
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Tipo { get; set; } = null!;
        public Boolean Estado { get; set; } = true;

        // Relación: ROL ||--o{ USUARIO
        public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    }
}