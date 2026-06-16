namespace Helpers
{
    public static class TicketEstadoNormalizer
    {
        public const string Abierto = "Abierto";
        public const string EnProceso = "En Proceso";
        public const string EnEspera = "En Espera";
        public const string Programado = "Programado";
        public const string PendienteConfirmacion = "Pendiente Confirmacion";
        public const string Reabierto = "Reabierto";
        public const string Cerrado = "Cerrado";
        public const string Resuelto = "Resuelto";

        public static string Normalize(string? estado)
        {
            if (string.IsNullOrWhiteSpace(estado))
                throw new System.ArgumentException("Estado de ticket no puede ser vacío. Use 'Abierto', 'En Espera', 'Programado' o 'Cerrado'.", nameof(estado));

            var trimmed = estado.Trim();
            var normalized = trimmed
                .Replace("_", " ")
                .Replace("-", " ")
                .Trim();

            if (trimmed.Equals(Abierto, System.StringComparison.OrdinalIgnoreCase))
                return Abierto;

            if (normalized.Equals("en espera", System.StringComparison.OrdinalIgnoreCase)
                || normalized.Equals("enespera", System.StringComparison.OrdinalIgnoreCase))
                return EnEspera;

            if (normalized.Equals("en proceso", System.StringComparison.OrdinalIgnoreCase)
                || normalized.Equals("enproceso", System.StringComparison.OrdinalIgnoreCase))
                return EnProceso;

            if (trimmed.Equals(Programado, System.StringComparison.OrdinalIgnoreCase))
                return Programado;

            if (normalized.Equals("pendiente confirmacion", System.StringComparison.OrdinalIgnoreCase)
                || normalized.Equals("pendienteconfirmacion", System.StringComparison.OrdinalIgnoreCase))
                return PendienteConfirmacion;

            if (trimmed.Equals(Reabierto, System.StringComparison.OrdinalIgnoreCase))
                return Reabierto;

            if (trimmed.Equals(Cerrado, System.StringComparison.OrdinalIgnoreCase))
                return Cerrado;

            if (trimmed.Equals(Resuelto, System.StringComparison.OrdinalIgnoreCase))
                return Resuelto;

            throw new System.ArgumentException("Estado de ticket inválido. Use 'Abierto', 'En Proceso', 'En Espera', 'Programado', 'Pendiente Confirmacion', 'Reabierto', 'Cerrado' o 'Resuelto'.", nameof(estado));
        }

        public static string? NormalizeOrNull(string? estado)
        {
            return estado == null ? null : Normalize(estado);
        }
    }
}
