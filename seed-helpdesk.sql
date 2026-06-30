BEGIN;

-- Mantener consistencia con la app (la API usa DateTime.UtcNow)
SET TIME ZONE 'UTC';

TRUNCATE TABLE 
public."Notificaciones",
public."HistorialTickets",
public."TicketComentarios",
public."TicketAsignados",
public."TicketAnexos",
public."Tickets",
public."TipoTickets",
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
-- TIPO TICKETS
-- =====================================================

INSERT INTO public."TipoTickets"
("Id", "Nombre")
VALUES
(1, 'Incidente'),
(2, 'Requerimiento');

-- =====================================================
-- CATEGORIAS
-- =====================================================

-- =====================================================
-- CATEGORIAS (Incidentes: TipoTicketId=1, Requerimientos: TipoTicketId=2)
-- =====================================================
-- Cada area de servicio se divide en categorias de Incidente y/o Requerimiento
-- segun la naturaleza del servicio.

INSERT INTO public."Categorias"
("Id", "AreaId", "TipoTicketId", "Nombre", "Descripcion", "Estado")
VALUES
-- INCIDENTES (TipoTicketId = 1)
(1,  1, 1, 'Usuarios y Accesos',         'Incidentes de inicio de sesion y autenticacion', TRUE),
(2,  1, 1, 'Correo Gmail City-Parking',  'Incidentes con correo Gmail City-Parking', TRUE),
(3,  1, 1, 'Correo Microsoft 365 Indigo','Incidentes con correo Microsoft 365 Indigo', TRUE),
(4,  1, 1, 'Correo MS 365 Central Parking','Incidentes con correo MS 365 Central Parking', TRUE),
(5,  6, 1, 'Hardware',                   'Fallas y diagnosticos de equipos', TRUE),
(6,  6, 1, 'Software',                   'Fallas de sistema operativo y aplicaciones', TRUE),
(7,  6, 1, 'Impresion',                  'Problemas de impresion y conectividad', TRUE),
(8,  5, 1, 'Conectividad',               'Problemas de red WiFi y cableada', TRUE),
(9,  5, 1, 'VPN',                        'Problemas de conexion VPN y acceso remoto', TRUE),
(10, 4, 1, 'Ciberseguridad',             'Incidentes de seguridad informatica', TRUE),
-- REQUERIMIENTOS (TipoTicketId = 2)
(11, 8, 2, 'Usuarios y Accesos',         'Gestion de cuentas, permisos y politicas', TRUE),
(12, 1, 2, 'Correo Gmail City-Parking',  'Configuracion y administracion Gmail', TRUE),
(13, 1, 2, 'Correo Microsoft 365 Indigo','Configuracion y soporte MS 365 Indigo', TRUE),
(14, 1, 2, 'Correo MS 365 Central Parking','Configuracion y soporte MS 365 Central Parking', TRUE),
(15, 6, 2, 'Hardware',                   'Instalacion de componentes y mantenimiento', TRUE),
(16, 6, 2, 'Software',                   'Instalacion y actualizacion de software', TRUE),
(17, 6, 2, 'Asignacion de Equipos',      'Preparacion y entrega de equipos de computo', TRUE),
(18, 2, 2, 'Organizacion',               'Reubicacion y adecuacion de puestos', TRUE),
(19, 6, 2, 'Impresion',                  'Configuracion de impresion y consumibles', TRUE),
(20, 5, 2, 'VPN',                        'Configuracion de conexiones VPN', TRUE),
(21, 4, 2, 'Ciberseguridad',             'Copias de seguridad y recuperacion', TRUE);

-- =====================================================
-- SUBCATEGORIAS
-- =====================================================
-- Cada subcategoria detalla los servicios especificos dentro de cada categoria.

INSERT INTO public."Subcategorias"
("Id", "CategoriaId", "NombreSubcategoria", "Descripcion", "Estado")
VALUES
-- ========== INCIDENTES ==========

-- Cat 1: Usuarios y Accesos (Incidente)
(1,  1, 'Soporte en inicio de sesion',    'Ayuda con problemas de inicio de sesion', TRUE),
(2,  1, 'Problemas de autenticacion',     'Incidentes de validacion y acceso', TRUE),

-- Cat 2: Correo Gmail City-Parking (Incidente)
(3,  2, 'Problemas de envio de correos',  'Dificultad para enviar correos Gmail', TRUE),
(4,  2, 'Problemas de recepcion',         'Dificultad para recibir correos Gmail', TRUE),

-- Cat 3: Correo Microsoft 365 Indigo (Incidente)
(5,  3, 'Problemas de sincronizacion',    'Sincronizacion de correo en Outlook', TRUE),
(6,  3, 'Problemas de acceso',            'Error al iniciar sesion en MS 365', TRUE),

-- Cat 4: Correo MS 365 Central Parking (Incidente)
(7,  4, 'Problemas de sincronizacion',    'Sincronizacion de correo en Outlook', TRUE),
(8,  4, 'Problemas de acceso',            'Error al iniciar sesion en MS 365', TRUE),

-- Cat 5: Hardware (Incidente)
(9,  5, 'Fallas de equipos de escritorio','Diagnostico y reparacion de PCs de escritorio', TRUE),
(10, 5, 'Fallas de equipos portatiles',   'Diagnostico y reparacion de laptops', TRUE),
(11, 5, 'Fallas de perifericos',          'Teclados, mouse, monitores y camaras', TRUE),
(12, 5, 'Mantenimiento correctivo',       'Reparacion de componentes dañados', TRUE),
(13, 5, 'Verificacion de compatibilidad', 'Pruebas de funcionamiento de hardware', TRUE),

-- Cat 6: Software (Incidente)
(14, 6, 'Fallas de sistema operativo',    'Errores y problemas del SO', TRUE),
(15, 6, 'Fallas de aplicaciones',         'Errores en software corporativo', TRUE),
(16, 6, 'Problemas de actualizaciones',   'Incidentes con parches y updates', TRUE),
(17, 6, 'Orientacion en uso de apps',     'Asesoria en uso de aplicaciones y plataformas', TRUE),

-- Cat 7: Impresion (Incidente)
(18, 7, 'Problemas de impresion',         'Fallas al imprimir documentos', TRUE),
(19, 7, 'Problemas de conectividad',      'Impresora no detectada o sin comunicacion', TRUE),

-- Cat 8: Conectividad (Incidente)
(20, 8, 'Problemas de WiFi',              'Conexion inalambrica lenta o sin acceso', TRUE),
(21, 8, 'Problemas de red cableada',      'Fallas en conexion ethernet', TRUE),
(22, 8, 'Sin internet',                   'Servicio de internet no disponible', TRUE),
(23, 8, 'Servicios de red no disponibles','Navegacion y recursos de red caidos', TRUE),

-- Cat 9: VPN (Incidente)
(24, 9, 'Problemas de conexion VPN',      'Error al establecer tunel VPN', TRUE),
(25, 9, 'Problemas de acceso remoto',     'Dificultad para conexion remota segura', TRUE),

-- Cat 10: Ciberseguridad (Incidente)
(26, 10, 'Incidentes de seguridad',       'Gestion de incidentes de seguridad informatica', TRUE),
(27, 10, 'Correos sospechosos o phishing','Analisis de correos fraudulentos', TRUE),
(28, 10, 'Cuentas comprometidas',         'Gestion de cuentas vulneradas', TRUE),

-- ========== REQUERIMIENTOS ==========

-- Cat 11: Usuarios y Accesos (Requerimiento)
(29, 11, 'Creacion de cuentas de usuario',   'Nuevas cuentas en el dominio', TRUE),
(30, 11, 'Modificacion de cuentas',          'Actualizacion de datos de usuario', TRUE),
(31, 11, 'Eliminacion de cuentas',           'Baja de usuarios del dominio', TRUE),
(32, 11, 'Restablecimiento de contrasenas',  'Reseteo de password de usuario', TRUE),
(33, 11, 'Asignacion de permisos y accesos', 'Accesos a carpetas y recursos de red', TRUE),
(34, 11, 'Configuracion de politicas',       'Politicas de seguridad y perfiles', TRUE),

-- Cat 12: Correo Gmail City-Parking (Requerimiento)
(35, 12, 'Configuracion de cuentas Gmail',     'Alta y configuracion de correo Gmail', TRUE),
(36, 12, 'Administracion de firmas',           'Creacion y modificacion de firmas', TRUE),
(37, 12, 'Filtros y reenvios de correo',       'Reglas de bandeja de entrada', TRUE),
(38, 12, 'Restablecimiento de contrasenas',    'Reseteo de password de Gmail', TRUE),
(39, 12, 'Capacitacion Gmail Workspace',       'Formacion basica en herramientas Gmail', TRUE),

-- Cat 13: Correo Microsoft 365 Indigo (Requerimiento)
(40, 13, 'Configuracion de cuentas Outlook',    'Alta y configuracion MS 365 en Outlook', TRUE),
(41, 13, 'Configuracion en dispositivos moviles','Correo en smartphones y tablets', TRUE),
(42, 13, 'Gestion de licencias',                'Asignacion y revision de licencias', TRUE),
(43, 13, 'Validacion de usuarios',              'Verificacion de accesos y credenciales', TRUE),
(44, 13, 'Apoyo en herramientas Office 365',    'Asistencia en uso de Office 365', TRUE),

-- Cat 14: Correo MS 365 Central Parking (Requerimiento)
(45, 14, 'Configuracion de cuentas Outlook',    'Alta y configuracion MS 365 en Outlook', TRUE),
(46, 14, 'Configuracion en dispositivos moviles','Correo en smartphones y tablets', TRUE),
(47, 14, 'Gestion de licencias',                'Asignacion y revision de licencias', TRUE),
(48, 14, 'Validacion de usuarios',              'Verificacion de accesos y credenciales', TRUE),
(49, 14, 'Apoyo en herramientas Office 365',    'Asistencia en uso de Office 365', TRUE),

-- Cat 15: Hardware (Requerimiento)
(50, 15, 'Instalacion de componentes',          'Memorias, discos duros y otros', TRUE),
(51, 15, 'Instalacion de dispositivos externos','Configuracion de perifericos nuevos', TRUE),
(52, 15, 'Mantenimiento preventivo',            'Limpieza y revision periodica', TRUE),

-- Cat 16: Software (Requerimiento)
(53, 16, 'Instalacion de sistemas operativos',  'Instalacion y actualizacion de SO', TRUE),
(54, 16, 'Instalacion de aplicaciones',         'Software corporativo y licenciado', TRUE),
(55, 16, 'Configuracion de software corporativo','Parametrizacion de apps empresariales', TRUE),
(56, 16, 'Instalacion de software complementario','Programas auxiliares requeridos', TRUE),
(57, 16, 'Aplicacion de parches de seguridad',  'Actualizaciones de seguridad', TRUE),

-- Cat 17: Asignacion de Equipos (Requerimiento)
(58, 17, 'Preparacion de equipos',              'Alistamiento segun perfil del usuario', TRUE),
(59, 17, 'Entrega de equipos',                  'Asignacion formal de equipo', TRUE),
(60, 17, 'Configuracion de cuentas y accesos',  'Correo y recursos corporativos', TRUE),
(61, 17, 'Instalacion de software requerido',   'Software necesario para el cargo', TRUE),

-- Cat 18: Organizacion (Requerimiento)
(62, 18, 'Reubicacion de equipos',              'Traslado de PCs y perifericos', TRUE),
(63, 18, 'Configuracion de conexiones',         'Red y energia en nueva ubicacion', TRUE),
(64, 18, 'Adecuacion de puesto de trabajo',     'Organizacion del espacio laboral', TRUE),

-- Cat 19: Impresion (Requerimiento)
(65, 19, 'Creacion de usuarios de impresion',   'Altas en sistema de impresion', TRUE),
(66, 19, 'Configuracion de accesos',            'Acceso a impresoras de red', TRUE),
(67, 19, 'Restablecimiento de claves',          'Reseteo de claves de impresion', TRUE),
(68, 19, 'Cambio de consumibles',               'Tonner, tinta y kits de mantenimiento', TRUE),
(69, 19, 'Capacitacion de impresoras',          'Uso basico de impresoras corporativas', TRUE),

-- Cat 20: VPN (Requerimiento)
(70, 20, 'Configuracion de VPN en equipos',      'VPN en equipos de escritorio y portatiles', TRUE),
(71, 20, 'Configuracion de VPN en dispositivos', 'VPN en smartphones y tablets', TRUE),

-- Cat 21: Ciberseguridad (Requerimiento)
(72, 21, 'Copias de seguridad',                 'Respaldo de informacion', TRUE),
(73, 21, 'Recuperacion de informacion',         'Restauracion de datos', TRUE),
(74, 21, 'Bloqueo y desbloqueo de cuentas',     'Gestion de cuentas comprometidas', TRUE);

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
        -- 1) Ticket en proceso (en tiempo) - Correo Gmail City-Parking Incidente
        (1, 5, 2, 3, 2, 1, 1, 'Error correo corporativo', 'No permite enviar correos', 'En Proceso', (NOW() - INTERVAL '30 minutes'), NULL::timestamptz, NULL::timestamptz, NULL::timestamptz, NULL::timestamptz, NULL::text, NULL::int),
        -- 2) Ticket en espera (SLA pausado) - Hardware Incidente
        (2, 5, 5, 9, 1, 2, 1, 'Servidor principal caido', 'El servidor no responde', 'En Espera', (NOW() - INTERVAL '90 minutes'), NULL::timestamptz, (NOW() - INTERVAL '10 minutes'), NULL::timestamptz, NULL::timestamptz, NULL::text, NULL::int),
        -- 3) Ticket vencido (SLA ya pasó) - Software Incidente
        (3, 5, 6, 15, 3, 3, 1, 'Error aplicacion web', 'La aplicacion genera error 500', 'En Proceso', (NOW() - INTERVAL '10 hours'), NULL::timestamptz, NULL::timestamptz, NULL::timestamptz, NULL::timestamptz, NULL::text, NULL::int),
        -- 4) Pendiente Confirmacion: agente solicito cierre, esperando usuario - Usuarios Requerimiento
        (4, 5, 11, 29, 2, 1, 2, 'Solicitud creacion usuario', 'Se requiere crear cuenta para nuevo empleado', 'Pendiente Confirmacion', (NOW() - INTERVAL '2 hours'), NULL::timestamptz, NULL::timestamptz, (NOW() - INTERVAL '15 minutes'), NULL::timestamptz, NULL::text, 2),
        -- 5) Reabierto: usuario rechazo el cierre - Software Incidente
        (5, 4, 6, 15, 3, 3, 1, 'Error modulo facturacion', 'El modulo de facturacion no carga correctamente', 'Reabierto', (NOW() - INTERVAL '5 hours'), NULL::timestamptz, NULL::timestamptz, (NOW() - INTERVAL '2 hours'), (NOW() - INTERVAL '1 hour'), 'El problema persiste, la facturacion sigue sin cargar', 3)
    ) AS v(
        "Id",
        "UsuarioCreadorId",
        "CategoriaId",
        "SubcategoriaId",
        "PrioridadId",
        "AreaId",
        "TipoTicketId",
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
        s."TipoTicketId",
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
    "TipoTicketId",
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
    t."TipoTicketId",
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

INSERT INTO public."TicketAsignados"
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

INSERT INTO public."TicketComentarios"
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

INSERT INTO public."TicketAnexos"
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
    'Activo'
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
    'Activo'
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
    'Activo'
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
-- NOTIFICACIONES
-- =====================================================

INSERT INTO public."Notificaciones"
(
    "Id",
    "UsuarioId",
    "TicketId",
    "Tipo",
    "Mensaje",
    "Leida",
    "FechaCreacion"
)
VALUES
(1, 2, 1, 'TICKET_CREADO', 'Nuevo ticket creado: Error correo corporativo', FALSE, NOW()),
(2, 3, 1, 'TICKET_CREADO', 'Nuevo ticket creado: Error correo corporativo', FALSE, NOW()),
(3, 2, 4, 'TICKET_ASIGNADO', 'Se te ha asignado el ticket: Solicitud creacion usuario', FALSE, NOW()),
(4, 5, 4, 'TICKET_ACTUALIZADO', 'El ticket ha sido actualizado: Solicitud creacion usuario', TRUE, NOW()),
(5, 5, 1, 'COMENTARIO_NUEVO', 'Carlos Ramirez comentó en: Error correo corporativo', FALSE, NOW());

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
