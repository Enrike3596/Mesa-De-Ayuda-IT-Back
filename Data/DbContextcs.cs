namespace Data
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Npgsql;
    using Models;

    public class DbContextcs : DbContext
    {
         public DbSet<Rol> Roles { get; set; }
         public DbSet<Usuario> Usuarios { get; set; }
         public DbSet<Ticket> Tickets { get; set; }
         public DbSet<Area> Areas { get; set; }
         public DbSet<Categoria> Categorias { get; set; }
         public DbSet<Subcategoria> Subcategorias { get; set; }
         public DbSet<Prioridad> Prioridades { get; set; }
         public DbSet<TicketAsignado> TicketAsignados { get; set; }
         public DbSet<TicketComentario> TicketComentarios { get; set; }
         public DbSet<TicketAnexo> TicketAnexos { get; set; }
         public DbSet<HistorialTicket> HistorialTickets { get; set; }
          public DbSet<TipoTicket> TipoTickets { get; set; }
          public DbSet<Notificacion> Notificaciones { get; set; }
        public DbContextcs(DbContextOptions<DbContextcs> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Ticket>(entity =>
            {
                entity.HasOne(t => t.CerradoPor)
                    .WithMany(u => u.TicketsCerrados)
                    .HasForeignKey(t => t.CerradoPorUsuarioId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(t => t.TicketTipo)
                    .WithMany(tt => tt.Tickets)
                    .HasForeignKey(t => t.TipoTicketId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Categoria>(entity =>
            {
                entity.HasOne(c => c.TicketTipo)
                    .WithMany(tt => tt.Categorias)
                    .HasForeignKey(c => c.TipoTicketId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return await base.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException pg
                                              && pg.SqlState == PostgresErrorCodes.UniqueViolation
                                              && pg.ConstraintName != null
                                              && pg.ConstraintName.StartsWith("PK_", StringComparison.OrdinalIgnoreCase))
            {
                await SyncAllPkSequencesAsync(cancellationToken);
                return await base.SaveChangesAsync(cancellationToken);
            }
        }

        private Task SyncAllPkSequencesAsync(CancellationToken cancellationToken)
        {
            const string sql = @"
DO $$
DECLARE
    r RECORD;
    max_id bigint;
BEGIN
    FOR r IN
        SELECT
            n.nspname AS schema_name,
            t.relname AS table_name,
            a.attname AS column_name,
            pg_get_serial_sequence(format('%I.%I', n.nspname, t.relname), a.attname) AS seq_name
        FROM pg_class t
        JOIN pg_namespace n ON n.oid = t.relnamespace
        JOIN pg_attribute a ON a.attrelid = t.oid
        JOIN pg_index i ON i.indrelid = t.oid AND i.indisprimary
        WHERE t.relkind = 'r'
          AND n.nspname = 'public'
          AND a.attnum = ANY(i.indkey)
          AND a.attnum > 0
          AND NOT a.attisdropped
          AND pg_get_serial_sequence(format('%I.%I', n.nspname, t.relname), a.attname) IS NOT NULL
    LOOP
        EXECUTE format('SELECT COALESCE(MAX(%I), 0) FROM %I.%I', r.column_name, r.schema_name, r.table_name)
            INTO max_id;

        EXECUTE format('SELECT setval(%L, %s, true)', r.seq_name, max_id);
    END LOOP;
END $$;";

            return Database.ExecuteSqlRawAsync(sql, cancellationToken);
        }
    }
}

