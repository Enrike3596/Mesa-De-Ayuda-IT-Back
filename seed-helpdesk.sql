BEGIN;

-- Mantener consistencia con la app (la API usa DateTime.UtcNow)
SET TIME ZONE 'UTC';

TRUNCATE TABLE 
public."HistorialTickets",
public."TkComentarios",
public."TkAsignados",
public."TkAnexos",
public."Tickets",
public."Subcategorias",
public."Categorias",
public."Usuarios",
public."Prioridades",
public."Roles",
public."Areas"
RESTART IDENTITY CASCADE;

INSERT INTO public."Areas"
("Id", "NombreArea", "Estado")
VALUES
(1, 'Mesa de Ayuda', TRUE),
(2, 'Infraestructura', TRUE),
(3, 'Desarrollo', TRUE),
(4, 'Seguridad Informatica', TRUE),
(5, 'Redes', TRUE),
(6, 'Soporte Tecnico', TRUE),
(7, 'Base de Datos', TRUE),
(8, 'Gestion TI', TRUE);

-- =====================================================
-- ROLES
-- =====================================================

INSERT INTO public."Roles"
("Id", "Nombre", "Tipo", "Estado")
VALUES
(1, 'Administrador', 'Sistema', TRUE),
(2, 'Agente de soporte técnico', 'AGENTE_TI', TRUE),
(3, 'Usuario', 'Final', TRUE);

-- =====================================================
-- PRIORIDADES
-- =====================================================

INSERT INTO public."Prioridades"
("Id", "Nombre", "Tipo", "Hora_sla", "Estado")
VALUES
(1, 'Critica', 'Alta', 2, TRUE),
(2, 'Alta', 'Alta', 4, TRUE),
(3, 'Media', 'Media', 8, TRUE),
(4, 'Baja', 'Baja', 24, TRUE);

-- =====================================================
-- CATEGORIAS
-- =====================================================

INSERT INTO public."Categorias"
("Id", "AreaId", "Nombre", "Descripcion", "Estado")
VALUES
(1, 1, 'Incidentes', 'Gestion de incidentes', TRUE),
(2, 1, 'Requerimientos', 'Solicitudes de usuarios', TRUE),
(3, 2, 'Servidores', 'Administracion de servidores', TRUE),
(4, 3, 'Aplicaciones', 'Problemas de software', TRUE),
(5, 4, 'Accesos', 'Gestion de accesos y permisos', TRUE),
(6, 5, 'Conectividad', 'Problemas de red', TRUE),
(7, 6, 'Hardware', 'Fallas de equipos', TRUE),
(8, 7, 'SQL', 'Administracion de BD', TRUE);

-- =====================================================
-- SUBCATEGORIAS
-- =====================================================

INSERT INTO public."Subcategorias"
("Id", "CategoriaId", "NombreSubcategoria", "Descripcion", "Estado")
VALUES
(1, 1, 'Correo Electronico', 'Problemas con correo', TRUE),
(2, 1, 'Impresoras', 'Fallas de impresion', TRUE),
(3, 2, 'Creacion Usuario', 'Solicitud de usuario', TRUE),
(4, 2, 'Cambio Equipo', 'Solicitud de cambio de PC', TRUE),
(5, 3, 'Servidor Caido', 'Servidor fuera de linea', TRUE),
(6, 3, 'Espacio Disco', 'Almacenamiento insuficiente', TRUE),
(7, 4, 'Frontend', 'Problemas interfaz', TRUE),
(8, 4, 'Backend', 'Problemas servidor', TRUE),
(9, 5, 'Bloqueo Usuario', 'Cuenta bloqueada', TRUE),
(10, 6, 'Sin Internet', 'Problemas conectividad', TRUE),
(11, 7, 'Pantalla', 'Daño de monitor', TRUE),
(12, 8, 'Consultas SQL', 'Errores SQL', TRUE);

-- =====================================================
-- USUARIOS
-- =====================================================

INSERT INTO public."Usuarios"
(
    "Id",
    "RolId",
    "AreaId",
    "Nombre",
    "Correo",
    "Telefono",
    "ContrasenaHash",
    "Estado",
    "FechaCreacion",
    "FechaModificacion"
)
VALUES
(
    1,
    1,
    8,
    'Administrador Sistema',
    'admin@helpdesk.local',
    '3000000001',
    'Admin123*',
    TRUE,
    NOW(),
    NOW()
),
(
    2,
    2,
    1,
    'Carlos Ramirez',
    'carlos.ramirez@helpdesk.local',
    '3000000002',
    'Carlos123*',
    TRUE,
    NOW(),
    NOW()
),
(
    3,
    2,
    2,
    'Laura Gomez',
    'laura.gomez@helpdesk.local',
    '3000000003',
    'Laura123*',
    TRUE,
    NOW(),
    NOW()
),
(
    4,
    3,
    3,
    'Andres Torres',
    'andres.torres@helpdesk.local',
    '3000000004',
    'Andres123*',
    TRUE,
    NOW(),
    NOW()
),
(
    5,
    3,
    1,
    'Sofia Martinez',
    'sofia.martinez@helpdesk.local',
    '3000000005',
    'Sofia123*',
    TRUE,
    NOW(),
    NOW()
);

-- =====================================================
-- TICKETS
-- =====================================================

-- Importante: "Estado" es string en el modelo (no boolean).
-- El SLA se calcula a partir de Prioridades.Hora_sla y se pausa si el estado es "En Espera" o "Programado".
WITH ticket_seed AS (
    SELECT * FROM (VALUES
        -- 1) Ticket en proceso (en tiempo)
        (1, 5, 1, 1, 2, 1, 'Error correo corporativo', 'No permite enviar correos', 'En Proceso', (NOW() - INTERVAL '30 minutes'), NULL::timestamptz, NULL::timestamptz, NULL::timestamptz, NULL::timestamptz, NULL::text, NULL::int),
        -- 2) Ticket en espera (SLA pausado)
        (2, 5, 3, 5, 1, 2, 'Servidor principal caido', 'El servidor no responde', 'En Espera', (NOW() - INTERVAL '90 minutes'), NULL::timestamptz, (NOW() - INTERVAL '10 minutes'), NULL::timestamptz, NULL::timestamptz, NULL::text, NULL::int),
        -- 3) Ticket vencido (SLA ya pasó)
        (3, 5, 4, 8, 3, 3, 'Error aplicacion web', 'La aplicacion genera error 500', 'En Proceso', (NOW() - INTERVAL '10 hours'), NULL::timestamptz, NULL::timestamptz, NULL::timestamptz, NULL::timestamptz, NULL::text, NULL::int),
        -- 4) Pendiente Confirmacion: agente solicito cierre, esperando usuario
        (4, 5, 2, 3, 2, 1, 'Solicitud creacion usuario', 'Se requiere crear cuenta para nuevo empleado', 'Pendiente Confirmacion', (NOW() - INTERVAL '2 hours'), NULL::timestamptz, NULL::timestamptz, (NOW() - INTERVAL '15 minutes'), NULL::timestamptz, NULL::text, 2),
        -- 5) Reabierto: usuario rechazo el cierre
        (5, 4, 4, 8, 3, 3, 'Error modulo facturacion', 'El modulo de facturacion no carga correctamente', 'Reabierto', (NOW() - INTERVAL '5 hours'), NULL::timestamptz, NULL::timestamptz, (NOW() - INTERVAL '2 hours'), (NOW() - INTERVAL '1 hour'), 'El problema persiste, la facturacion sigue sin cargar', 3)
    ) AS v(
        "Id",
        "UsuarioCreadorId",
        "CategoriaId",
        "SubcategoriaId",
        "PrioridadId",
        "AreaId",
        "Titulo",
        "Descripcion",
        "Estado",
        "FechaCreacion",
        "FechaCierre",
        "FechaPausaSLA",
        "FechaSolicitudCierre",
        "FechaConfirmacionCierre",
        "MotivoRechazo",
        "CerradoPorUsuarioId"
    )
), ticket_calc AS (
    SELECT
        s."Id",
        s."UsuarioCreadorId",
        s."CategoriaId",
        s."SubcategoriaId",
        s."PrioridadId",
        s."AreaId",
        s."Titulo",
        s."Descripcion",
        s."Estado",
        s."FechaCreacion",
        s."FechaCierre",
        s."FechaPausaSLA",
        s."FechaSolicitudCierre",
        s."FechaConfirmacionCierre",
        s."MotivoRechazo",
        s."CerradoPorUsuarioId",
        p."Hora_sla" AS horas_sla,
        CASE
            WHEN s."FechaPausaSLA" IS NULL THEN 0
            ELSE GREATEST(0, FLOOR(EXTRACT(EPOCH FROM (NOW() - s."FechaPausaSLA")) / 60))::int
        END AS minutos_pausa_actual
    FROM ticket_seed s
    JOIN public."Prioridades" p ON p."Id" = s."PrioridadId"
)
INSERT INTO public."Tickets"
(
    "Id",
    "UsuarioCreadorId",
    "CategoriaId",
    "SubcategoriaId",
    "PrioridadId",
    "AreaId",
    "Titulo",
    "Descripcion",
    "Estado",
    "FechaCreacion",
    "FechaCierre",
    "FechaLimiteSLA",
    "FechaPausaSLA",
    "TiempoAcumuladoPausaMinutos",
    "SLAVencido",
    "EstadoSLA",
    "FechaSolicitudCierre",
    "FechaConfirmacionCierre",
    "MotivoRechazo",
    "CerradoPorUsuarioId"
)
SELECT
    t."Id",
    t."UsuarioCreadorId",
    t."CategoriaId",
    t."SubcategoriaId",
    t."PrioridadId",
    t."AreaId",
    t."Titulo",
    t."Descripcion",
    t."Estado",
    t."FechaCreacion",
    t."FechaCierre",
    (t."FechaCreacion" + (t.horas_sla::text || ' hours')::interval + (t.minutos_pausa_actual::text || ' minutes')::interval) AS "FechaLimiteSLA",
    t."FechaPausaSLA",
    CASE WHEN t."FechaPausaSLA" IS NULL THEN 0 ELSE t.minutos_pausa_actual END AS "TiempoAcumuladoPausaMinutos",
    CASE WHEN t."FechaPausaSLA" IS NULL AND NOW() > (t."FechaCreacion" + (t.horas_sla::text || ' hours')::interval) THEN TRUE ELSE FALSE END AS "SLAVencido",
    CASE
        WHEN t."Estado" = 'Cerrado' OR t."Estado" = 'Resuelto' THEN 'Resuelto En Tiempo'
        WHEN t."Estado" = 'Pendiente Confirmacion' THEN 'En Tiempo'
        WHEN t."FechaPausaSLA" IS NOT NULL THEN 'Pausado'
        WHEN NOW() > (t."FechaCreacion" + (t.horas_sla::text || ' hours')::interval) THEN 'Vencido'
        ELSE 'En Tiempo'
    END AS "EstadoSLA",
    t."FechaSolicitudCierre",
    t."FechaConfirmacionCierre",
    t."MotivoRechazo",
    t."CerradoPorUsuarioId"
FROM ticket_calc t;

-- =====================================================
-- TICKETS ASIGNADOS
-- =====================================================

INSERT INTO public."TkAsignados"
(
    "Id",
    "TicketId",
    "UsuarioAgenteId",
    "FechaAsignacion"
)
VALUES
(1, 1, 2, NOW()),
(2, 2, 3, NOW()),
(3, 3, 4, NOW()),
(4, 4, 2, NOW()),
(5, 5, 3, NOW());

-- =====================================================
-- COMENTARIOS TICKETS
-- =====================================================

INSERT INTO public."TkComentarios"
(
    "Id",
    "TicketId",
    "UsuarioId",
    "Comentario",
    "FechaCreacion",
    "EsInterno"
)
VALUES
(
    1,
    1,
    2,
    'Se valida configuracion de correo.',
    NOW(),
    FALSE
),
(
    2,
    1,
    1,
    'Revisar si el usuario tiene permisos SMTP habilitados.',
    NOW(),
    TRUE
),
(
    3,
    2,
    3,
    'Servidor reiniciado correctamente.',
    NOW(),
    FALSE
),
(
    4,
    2,
    3,
    'El servidor tenia 45 dias de uptime, posible falla de memoria.',
    NOW(),
    TRUE
),
(
    5,
    3,
    4,
    'Se revisa error en backend.',
    NOW(),
    FALSE
),
(
    6,
    3,
    4,
    'El error 500 viene de un timeout en la conexion a BD.',
    NOW(),
    TRUE
);

-- =====================================================
-- ANEXOS TICKETS
-- =====================================================

INSERT INTO public."TkAnexos"
(
    "Id",
    "TicketId",
    "UsuarioId",
    "NombreArchivo",
    "TipoArchivo",
    "TamanoArchivo",
    "UrlArchivo",
    "FechaCarga",
    "Estado"
)
VALUES
(
    1,
    1,
    5,
    'error_correo.png',
    'image/png',
    2048,
    '/uploads/error_correo.png',
    NOW(),
    TRUE
),
(
    2,
    2,
    5,
    'servidor_logs.txt',
    'text/plain',
    4096,
    '/uploads/servidor_logs.txt',
    NOW(),
    TRUE
),
(
    3,
    3,
    5,
    'error_backend.pdf',
    'application/pdf',
    5096,
    '/uploads/error_backend.pdf',
    NOW(),
    TRUE
);

-- =====================================================
-- HISTORIAL TICKETS (FLUJO DE CIERRE)
-- =====================================================

INSERT INTO public."HistorialTickets"
(
    "Id",
    "TicketId",
    "UsuarioId",
    "Descripcion",
    "FechaAccion"
)
VALUES
-- Ticket 4: Agente solicita cierre
(1, 4, 2, 'Cierre solicitado. Solución: Usuario creado y permisos asignados correctamente.', (NOW() - INTERVAL '15 minutes')),
-- Ticket 5: Agente solicita cierre
(2, 5, 3, 'Cierre solicitado. Solución: Se reinicio el servicio de facturacion.', (NOW() - INTERVAL '2 hours')),
-- Ticket 5: Usuario rechaza cierre
(3, 5, 4, 'Usuario rechazó el cierre. Motivo: El problema persiste, la facturacion sigue sin cargar.', (NOW() - INTERVAL '1 hour'));

-- =====================================================
-- SINCRONIZAR SECUENCIAS (POST-SEED)
-- =====================================================

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
END $$;

COMMIT;
