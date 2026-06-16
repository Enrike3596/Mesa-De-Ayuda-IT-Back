using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Middleware
{
    public sealed class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ArgumentException ex)
            {
                if (context.Response.HasStarted) throw;

                context.Response.Clear();
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Response.ContentType = "application/problem+json";

                var problem = new ProblemDetails
                {
                    Status = context.Response.StatusCode,
                    Title = "Validación inválida",
                    Detail = ex.Message,
                    Instance = context.Request.Path
                };

                var json = JsonSerializer.Serialize(problem);
                await context.Response.WriteAsync(json);
            }
            catch (InvalidOperationException ex)
            {
                if (context.Response.HasStarted) throw;

                context.Response.Clear();
                context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                context.Response.ContentType = "application/problem+json";

                var problem = new ProblemDetails
                {
                    Status = context.Response.StatusCode,
                    Title = "Conflicto de negocio",
                    Detail = ex.Message,
                    Instance = context.Request.Path
                };

                var json = JsonSerializer.Serialize(problem);
                await context.Response.WriteAsync(json);
            }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException pg
                                             && pg.SqlState == PostgresErrorCodes.UniqueViolation)
            {
                if (context.Response.HasStarted) throw;

                context.Response.Clear();
                context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                context.Response.ContentType = "application/problem+json";

                var detail = "Registro duplicado (violación de unicidad).";
                if (!string.IsNullOrWhiteSpace(pg.ConstraintName))
                    detail += $" Restricción: {pg.ConstraintName}.";

                var problem = new ProblemDetails
                {
                    Status = context.Response.StatusCode,
                    Title = "Conflicto al guardar",
                    Detail = detail,
                    Instance = context.Request.Path
                };

                var json = JsonSerializer.Serialize(problem);
                await context.Response.WriteAsync(json);
            }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException pg
                                             && pg.SqlState == PostgresErrorCodes.ForeignKeyViolation)
            {
                if (context.Response.HasStarted) throw;

                context.Response.Clear();
                context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                context.Response.ContentType = "application/problem+json";

                var detail = "Operación rechazada: el registro relacionado no existe.";
                if (!string.IsNullOrWhiteSpace(pg.ConstraintName))
                    detail += $" Restricción: {pg.ConstraintName}.";

                var problem = new ProblemDetails
                {
                    Status = context.Response.StatusCode,
                    Title = "Error de integridad referencial",
                    Detail = detail,
                    Instance = context.Request.Path
                };

                var json = JsonSerializer.Serialize(problem);
                await context.Response.WriteAsync(json);
            }
        }
    }
}
