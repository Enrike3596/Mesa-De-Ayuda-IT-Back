using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Middleware;
using Repositories;
using Services;
using Hubs;

var builder = WebApplication.CreateBuilder(args);

// =========================
// 1) Infraestructura / BD
// =========================
// Configurar EF Core con PostgreSQL
builder.Services.AddDbContext<Data.DbContextcs>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// =============================================
// 2) Inyección de dependencias (DI)
//    Flujo: Controller → Service → Repository
// =============================================
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();

builder.Services.AddScoped<IRolRepository, RolRepository>();
builder.Services.AddScoped<IRolService, RolService>();

builder.Services.AddScoped<IAreaRepository, AreaRepository>();
builder.Services.AddScoped<IAreaService, AreaService>();

builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();

builder.Services.AddScoped<IPrioridadRepository, PrioridadRepository>();
builder.Services.AddScoped<IPrioridadService, PrioridadService>();

builder.Services.AddScoped<ITicketRepository, TicketRepository>();
builder.Services.AddScoped<ITicketService, TicketService>();

builder.Services.AddScoped<ITicketAsignadoRepository, TicketAsignadoRepository>();
builder.Services.AddScoped<ITicketAsignadoService, TicketAsignadoService>();

builder.Services.AddScoped<ITicketComentarioRepository, TicketComentarioRepository>();
builder.Services.AddScoped<ITicketComentarioService, TicketComentarioService>();

builder.Services.AddScoped<ITicketAnexosRepository, TicketAnexosRepository>();
builder.Services.AddScoped<ITicketAnexoService, TicketAnexoService>();

builder.Services.AddSingleton<IFileStorageService, AzureBlobStorageService>();

builder.Services.AddScoped<ISubcategoriaRepository, SubcategoriaRepository>();
builder.Services.AddScoped<ISubcategoriaService, SubcategoriaService>();

builder.Services.AddScoped<ITipoTicketRepository, TipoTicketRepository>();
builder.Services.AddScoped<ITipoTicketService, TipoTicketService>();

builder.Services.AddScoped<INotificacionService, NotificacionService>();

// =========================
// 3) SignalR (Tiempo real)
// =========================
builder.Services.AddSignalR();

// =========================
// 4) MVC / Controllers
// =========================
builder.Services.AddControllers();
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errores = context.ModelState
            .Where(x => x.Value?.Errors.Count > 0)
            .ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
            );

        return new BadRequestObjectResult(new
        {
            message = "Error de validación en la solicitud.",
            errors = errores
        });
    };
});
builder.Services.AddEndpointsApiExplorer();

// =========================
// 5) Swagger (con soporte JWT)
// =========================
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "HelpDesk API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingresa: Bearer {token}"
    });
    c.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", doc, externalResource: null)] = new List<string>()
    });
});

// =========================
// 6) Autenticación / Autorización (JWT)
// =========================
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer           = true,
            ValidateAudience         = true,
            ValidateLifetime         = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer              = builder.Configuration["Jwt:Issuer"],
            ValidAudience            = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey         = new SymmetricSecurityKey(
                                         Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };

        // SignalR envía el token JWT como query string (access_token) en WebSockets
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                if (!string.IsNullOrEmpty(accessToken))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

// =========================
// 7) CORS (Frontend React + SignalR)
// =========================
builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactApp", policy =>
        policy.WithOrigins(
                  "http://localhost:5173", // Vite
                  "http://localhost:3000", // Next.js
                  "https://victorious-field-01396c30f.7.azurestaticapps.net") // Azure SWA
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials()); // Requerido por SignalR (WebSockets / LongPolling)
});

var app = builder.Build();

// =========================
// 8) Inicialización de BD
// =========================
// Crear la base de datos y las tablas automáticamente si no existen
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<Data.DbContextcs>();
    context.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// =========================
// 9) Middleware / Pipeline
// =========================
// Manejo centralizado de errores de validación (ArgumentException → 400 ProblemDetails)
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseCors("ReactApp");

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<TicketHub>("/hubs/ticket").RequireAuthorization();
app.Run();
