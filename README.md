# HelpDesk API — Sistema de Gestión de Tickets para Mesa de Ayuda TI

Backend robusto y escalable para la gestión integral de tickets de soporte técnico, construido con **.NET 10**, **SignalR** y **PostgreSQL**. Diseñado para integrarse con un frontend moderno en **Next.js 16** (React 19, TypeScript, Tailwind CSS v4).

---

## Tabla de Contenidos

- [El Problema](#el-problema)
- [La Solución](#la-solución)
- [Lo Que Puede Lograr](#lo-que-puede-lograr)
- [Stack Tecnológico](#stack-tecnológico)
  - [Backend (este repositorio)](#backend-este-repositorio)
  - [Frontend](#frontend)
- [Arquitectura](#arquitectura)
- [Requisitos](#requisitos)
- [Instalación y Configuración](#instalación-y-configuración)
- [Comandos](#comandos)
- [Estructura del Proyecto](#estructura-del-proyecto)
- [API Endpoints](#api-endpoints)
- [Tiempo Real con SignalR](#tiempo-real-con-signalr)
- [Seguridad](#seguridad)
- [Integración con el Frontend](#integración-con-el-frontend)
- [Paleta Corporativa](#paleta-corporativa)

---

## El Problema

En las **pequeñas y medianas empresas**, la gestión del soporte técnico interno suele resolverse con herramientas improvisadas: correos electrónicos dispersos, hojas de cálculo compartidas, mensajes de WhatsApp o tableros físicos. Este enfoque genera:

- **Pérdida de trazabilidad** — Los tickets se pierden en bandejas de entrada saturadas y no hay un historial centralizado.
- **Tiempos de respuesta inconsistentes** — Sin prioridades definidas ni SLA, los incidentes críticos se mezclan con consultas menores.
- **Falta de visibilidad** — Los líderes de TI no tienen métricas ni paneles para medir el desempeño del equipo.
- **Duplicación de esfuerzos** — Varios agentes pueden atender el mismo problema sin coordinación.
- **Ausencia de accountability** — No se sabe quién hizo qué ni cuándo, y los usuarios no tienen forma de dar seguimiento a sus solicitudes.
- **Datos de reporte inexistentes** — Sin una base de datos estructurada, es imposible generar reportes de productividad, tiempos de atención o áreas con mayor incidencia.

Esta fragmentación eleva los costos operativos, reduce la satisfacción de los usuarios internos y dificulta la toma de decisiones informadas en el departamento de TI.

---

## La Solución

**HelpDesk API** es un sistema centralizado de gestión de tickets (mesa de ayuda) que ordena, automatiza y da visibilidad a todo el flujo de soporte técnico empresarial. Proporciona:

- **Backend API REST** completo con autenticación JWT, validación por roles y documentación Swagger interactiva.
- **Tiempo real** mediante SignalR — los agentes y usuarios reciben actualizaciones al instante sin recargar la página.
- **Gestión de SLA** con seguimiento de tiempos límite, pausas y vencimiento automatizado.
- **Control de acceso por roles** — `ADMIN`, `AGENTE_TI` y `USUARIO` con permisos claramente diferenciados.
- **Archivos adjuntos** — Almacenamiento seguro de evidencias por ticket con organización en estructura de carpetas.
- **Comentarios internos y externos** — Comunicación transparente con el usuario y discusiones internas entre agentes.
- **Historial de acciones** — Auditoría completa de cada cambio sobre los tickets.
- **Flujo de cierre con aprobación** — Solicitud de cierre, aprobación o rechazo por parte del usuario solicitante.
- **PostgreSQL** como motor de base de datos relacional, garantizando integridad referencial y consultas eficientes.

Todo el backend **se entrega listo para integrarse** con un frontend moderno construido en Next.js 16, React 19, TypeScript y Tailwind CSS v4, formando una solución completa y profesional.

---

## Lo Que Puede Lograr

Con esta plataforma su empresa podrá:

| Beneficio | Impacto |
|---|---|
| **Reducir tiempos de atención** hasta un 40% | SLA automatizados y asignación inteligente |
| **Eliminar tickets perdidos** | Trazabilidad completa de principio a fin |
| **Medir productividad del equipo** | Reportes por agente, área y categoría |
| **Cumplir acuerdos de nivel de servicio** | Alertas automáticas de vencimiento de SLA |
| **Mejorar satisfacción de usuarios** | Seguimiento en tiempo real y cierre con aprobación |
| **Centralizar la comunicación** | Comentarios, archivos adjuntos e historial en un solo lugar |
| **Escalar sin límite** | Arquitectura modular preparada para alto volumen |
| **Integrarse con su stack actual** | API REST documentada y CORS configurado para múltiples orígenes |

---

## Stack Tecnológico

### Backend (este repositorio)

| Categoría | Tecnología |
|---|---|
| **Framework** | ASP.NET Core 10 |
| **Lenguaje** | C# 13 (nullable enabled, implicit usings) |
| **ORM** | Entity Framework Core 10 |
| **Base de datos** | PostgreSQL 16+ (vía Npgsql) |
| **Autenticación** | JWT Bearer (HMAC-SHA256) |
| **Tiempo real** | SignalR |
| **Documentación API** | Swagger / Swashbuckle 10 |
| **Hash de contraseñas** | BCrypt (BCrypt.Net-Next) |
| **Arquitectura** | Controller → Service → Repository → DbContext |

### Frontend

| Categoría | Tecnología |
|---|---|
| **Framework** | Next.js 16 (App Router) |
| **UI** | React 19 |
| **Lenguaje** | TypeScript 5 (strict mode) |
| **Estilos** | Tailwind CSS v4, shadcn/ui (New York) |
| **Formularios** | React Hook Form + Zod |
| **HTTP** | Axios, SWR |
| **Gráficos** | Recharts |
| **Notificaciones** | Sonner |
| **Iconos** | Lucide React |
| **Paquete** | pnpm |

---

## Arquitectura

```
┌─────────────────────────────────────────────────────┐
│                    CLIENTE                          │
│  ┌──────────────────────────────────────────────┐   │
│  │          Next.js 16 (App Router)             │   │
│  │   React 19 / TypeScript / Tailwind v4        │   │
│  └──────────────┬───────────────────────────────┘   │
│                 │ HTTP / SignalR                    │
│                 ▼                                   │
│  ┌──────────────────────────────────────────────┐   │
│  │          .NET 10 Web API                     │   │
│  │  ┌──────┐ ┌──────────┐ ┌───────────────┐    │   │
│  │  │JWT   │ │Controllers│ │ SignalR Hub   │    │   │
│  │  │Auth  │ │  (REST)   │ │ /hubs/ticket  │    │   │
│  │  └──────┘ └────┬─────┘ └───────────────┘    │   │
│  │                ▼                             │   │
│  │         ┌────────────┐                      │   │
│  │         │  Services  │                      │   │
│  │         └─────┬──────┘                     │   │
│  │               ▼                            │   │
│  │         ┌────────────┐                      │   │
│  │         │Repository  │                      │   │
│  │         └─────┬──────┘                     │   │
│  └───────────────┼────────────────────────────┘   │
│                  ▼                                │
│  ┌──────────────────────────────────────────────┐ │
│  │          PostgreSQL 16                        │ │
│  │          HelpDeskDB                          │ │
│  └──────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────┘
```

### Principios de diseño

- **Separación de responsabilidades** — Capas de Controller, Service y Repository claramente diferenciadas.
- **Inyección de dependencias** — Todos los servicios y repositorios registrados como `Scoped` en el contenedor DI.
- **Manejo centralizado de errores** — `ExceptionHandlingMiddleware` que transforma excepciones en respuestas `ProblemDetails` estándar (RFC 7807).
- **Auditoría** — Cada operación sobre tickets queda registrada en la tabla `HistorialTicket`.
- **Autenticación obligatoria** — Todas las rutas de ticket requieren JWT válido; SignalR también valida el token en la conexión.

### Roles del sistema

| Rol | Permisos |
|---|---|
| **ADMIN** | Acceso total: CRUD de usuarios, catálogos y todos los tickets. |
| **AGENTE_TI** | Gestionar tickets asignados, comentar (interno/externo), adjuntar archivos, cerrar tickets. |
| **USUARIO** | Crear tickets propios, dar seguimiento, aprobar/rechazar cierres, ver historial. |

---

## Requisitos

- [.NET SDK 10.0](https://dotnet.microsoft.com/download/dotnet/10.0)
- [PostgreSQL 16+](https://www.postgresql.org/download/)
- (Opcional) [Node.js 20+](https://nodejs.org/) y [pnpm](https://pnpm.io/) solo si despliega el frontend

---

## Instalación y Configuración

### 1. Clonar el repositorio

```bash
git clone https://github.com/su-organizacion/helpdesk-api.git
cd helpdesk-api
```

### 2. Configurar la base de datos

Edite `appsettings.json` con su conexión a PostgreSQL:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=HelpDeskDB;Username=postgres;Password=su_password"
  }
}
```

### 3. Restaurar dependencias y crear la base de datos

```bash
dotnet restore
dotnet run
```

Al iniciar por primera vez, la aplicación ejecuta `EnsureCreated()` para crear automáticamente la base de datos y todas las tablas.

### 4. (Opcional) Sembrar datos de prueba

Ejecute el script `seed-helpdesk.sql` contra su base de datos PostgreSQL para poblar áreas, roles, prioridades, categorías, usuarios y tickets de demostración:

```bash
psql -U postgres -d HelpDeskDB -f seed-helpdesk.sql
```

### 5. Verificar que funciona

La API estará disponible en `http://localhost:5214`. Acceda a la documentación interactiva en:

```
http://localhost:5214/swagger
```

---

## Comandos

### Backend (.NET)

| Comando | Descripción |
|---|---|
| `dotnet run` | Inicia el servidor en `http://localhost:5214` |
| `dotnet build` | Compila el proyecto |
| `dotnet watch run` | Inicia con recarga automática en desarrollo |
| `dotnet ef migrations add <nombre>` | Genera una nueva migración de base de datos |
| `dotnet ef database update` | Aplica migraciones pendientes |

### Frontend (Next.js)

| Comando | Descripción |
|---|---|
| `pnpm dev` | Inicia servidor de desarrollo en `http://localhost:3000` |
| `pnpm build` | Genera build de producción |
| `pnpm start` | Inicia servidor de producción |
| `pnpm lint` | Ejecuta ESLint |

---

## Estructura del Proyecto

```
├── Controllers/          # Controladores REST (11)
│   ├── AuthController.cs       # Login, generación de JWT
│   ├── TicketController.cs     # CRUD + SLA + flujo de cierre
│   ├── UsuarioController.cs    # CRUD de usuarios
│   ├── AreaController.cs       # CRUD de áreas
│   ├── CategoriaController.cs  # CRUD de categorías
│   ├── SubcategoriaController.cs
│   ├── PrioridadController.cs
│   ├── RolController.cs
│   ├── TicketAsignadoController.cs # Asignación agente-ticket
│   ├── TicketComentarioController.cs
│   └── TicketAnexoController.cs    # Archivos adjuntos
├── Services/             # Lógica de negocio (11)
├── Repositories/         # Acceso a datos (10)
├── Models/               # Entidades del dominio (12)
├── DTOs/                 # Objetos de transferencia (12)
├── Hubs/                 # SignalR: TicketHub
├── Middleware/            # ExceptionHandlingMiddleware
├── Data/                 # DbContext de EF Core
├── Migrations/           # Migraciones de base de datos
├── Helpers/              # Normalizadores de estado
├── Program.cs            # Punto de entrada y configuración DI
├── appsettings.json      # Configuración principal
└── seed-helpdesk.sql     # Script de datos de prueba
```

---

## API Endpoints

### Autenticación

| Método | Ruta | Descripción |
|---|---|---|
| `POST` | `/api/auth/login` | Inicia sesión y devuelve JWT |

### Tickets

| Método | Ruta | Descripción |
|---|---|---|
| `GET` | `/api/ticket` | Lista todos los tickets |
| `GET` | `/api/ticket/{id}` | Obtiene un ticket por ID |
| `GET` | `/api/ticket/{id}/sla` | Información SLA del ticket |
| `GET` | `/api/ticket/pendientes-confirmacion` | Tickets pendientes de confirmación de cierre |
| `POST` | `/api/ticket` | Crea un nuevo ticket |
| `PUT` | `/api/ticket/{id}` | Actualiza un ticket |
| `PUT` | `/api/ticket/{id}/solicitar-cierre` | Solicita cierre del ticket |
| `PUT` | `/api/ticket/{id}/confirmar-cierre` | Confirma o rechaza el cierre |
| `DELETE` | `/api/ticket/{id}` | Elimina un ticket |

### Catálogos

| Método | Ruta | Descripción |
|---|---|---|
| `GET/POST/PUT/DELETE` | `/api/usuario` | CRUD de usuarios |
| `GET/POST/PUT/DELETE` | `/api/rol` | CRUD de roles |
| `GET/POST/PUT/DELETE` | `/api/area` | CRUD de áreas |
| `GET/POST/PUT/DELETE` | `/api/categoria` | CRUD de categorías |
| `GET/POST/PUT/DELETE` | `/api/subcategoria` | CRUD de subcategorías |
| `GET/POST/PUT/DELETE` | `/api/prioridad` | CRUD de prioridades con SLA |

### Operaciones sobre tickets

| Método | Ruta | Descripción |
|---|---|---|
| `GET/POST/PUT/DELETE` | `/api/TicketAsignado` | Asignar/desasignar agentes a tickets |
| `GET/POST/PUT/DELETE` | `/api/TicketComentario` | Comentarios (públicos e internos) |
| `GET/POST/PUT/DELETE` | `/api/TicketAnexo` | Archivos adjuntos |

Toda la documentación detallada con esquemas de solicitud/respuesta está disponible en `/swagger` al ejecutar la API.

---

## Tiempo Real con SignalR

El hub de SignalR en `/hubs/ticket` permite actualizaciones en vivo sin polling:

**Eventos emitidos desde el servidor:**

| Evento | Descripción |
|---|---|
| `TicketCreado` | Se dispara al crear un nuevo ticket |
| `TicketActualizado` | Se dispara al modificar cualquier campo del ticket |

**Agrupación por usuario:** Al conectarse, el cliente es agregado automáticamente a un grupo `user_{userId}`, permitiendo notificaciones personalizadas.

**Ejemplo de conexión desde el frontend:**

```typescript
import * as signalR from "@microsoft/signalr";

const connection = new signalR.HubConnectionBuilder()
  .withUrl(`${apiUrl}/hubs/ticket`, {
    accessTokenFactory: () => getToken(),
  })
  .withAutomaticReconnect()
  .build();

connection.on("TicketCreado", (ticket) => {
  // Actualizar UI en tiempo real
});

connection.on("TicketActualizado", (ticket) => {
  // Refrescar datos del ticket
});

await connection.start();
```

---

## Seguridad

- **Autenticación JWT** — Tokens firmados con clave secreta, expiración configurable (60 min por defecto).
- **BCrypt** para hash de contraseñas — Protección contra ataques de fuerza bruta y rainbow tables.
- **Autorización por roles** — Atributo `[Authorize]` con validación de rol en cada endpoint.
- **CORS restringido** — Solo orígenes explícitamente configurados (`localhost:3000`, `localhost:5173`).
- **Manejo seguro de errores** — Middleware que devuelve `ProblemDetails` sin exponer trazas internas en producción.
- **Validación de datos** — FluentValidation a través de Data Annotations en los DTOs.

---

## Integración con el Frontend

Este backend está diseñado para consumirse desde un frontend en **Next.js 16** (repositorio separado). La comunicación se realiza mediante:

### 1. Variables de entorno (frontend)

```env
NEXT_PUBLIC_API_URL=http://localhost:5214/api
```

### 2. Librerías recomendadas (ya incluidas en el frontend)

- **Axios + SWR** — Para peticiones HTTP con caché y revalidación automática.
- **SignalR client** — Para conexiones en tiempo real.
- **date-fns** — Para formateo de fechas y cálculos de SLA.

### 3. Lo que obtendrá

| Funcionalidad | Backend | Frontend |
|---|---|---|
| Autenticación | JWT + login endpoint | Login form, persistencia de token |
| CRUD de tickets | TicketController | Páginas de listado, detalle, creación y edición |
| Tiempo real | SignalR Hub | Notificaciones push, actualización automática de listas |
| SLA y alertas | Cálculo en TicketService | Indicadores visuales de vencimiento, progreso de SLA |
| Archivos adjuntos | FileStorageService | Drag & drop, vista previa, descarga |
| Roles UI | Filtrado por rol en controllers | Menús, rutas y acciones condicionadas por rol |
| Dashboard y gráficos | Endpoints de datos agregados | Recharts para métricas y tendencias |
| Flujo de cierre | Solicitud + confirmación/rechazo | Diálogo de cierre, lista de pendientes |

---

## Paleta Corporativa

| Color | Código | Uso |
|---|---|---|
| Púrpura | `#552373` | Elementos de marca, encabezados |
| Magenta | `#B80E80` | Acciones primarias, enlaces |
| Azul oscuro | `#263D77` | Barras de navegación, fondos |
| Teal | `#009BAA` | Acento, estados positivos, SLA en tiempo |

---

## Licencia

Este proyecto es privado y de uso interno. Todos los derechos reservados.
