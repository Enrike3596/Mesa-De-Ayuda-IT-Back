namespace Helpers
{
    public static class EstadoNormalizer
    {
        public const string Activo = "Activo";
        public const string Inactivo = "Inactivo";

        public static string Normalize(string? estado)
        {
            if (string.IsNullOrWhiteSpace(estado))
            {
                throw new System.ArgumentException("Estado no puede ser vacío. Use 'Activo' o 'Inactivo'.", nameof(estado));
            }

            var trimmed = estado.Trim();

            if (trimmed.Equals(Activo, System.StringComparison.OrdinalIgnoreCase) || trimmed.Equals("active", System.StringComparison.OrdinalIgnoreCase))
                return Activo;

            if (trimmed.Equals(Inactivo, System.StringComparison.OrdinalIgnoreCase) || trimmed.Equals("inactive", System.StringComparison.OrdinalIgnoreCase))
                return Inactivo;

            throw new System.ArgumentException("Estado inválido. Use 'Activo' o 'Inactivo'.", nameof(estado));
        }

        public static string? NormalizeOrNull(string? estado)
        {
            return estado == null ? null : Normalize(estado);
        }
    }
}
