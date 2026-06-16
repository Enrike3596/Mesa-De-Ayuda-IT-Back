using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Data;
using Models;
using DTOs;

namespace Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly DbContextcs _context;
        private readonly IConfiguration _config;

        public AuthController(DbContextcs context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // POST api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            if (dto == null)
                return BadRequest(new { message = "Body requerido" });

            // 1. Buscar usuario en BD
            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .Include(u => u.Area)
                .FirstOrDefaultAsync(u => u.Correo == dto.Correo);

            if (usuario == null)
                return Unauthorized(new { message = "Credenciales inválidas" });

            // 2. Verificar contraseña
            // Nota: tu BD puede estar guardando la contraseña en texto plano (o con datos corruptos).
            // BCrypt.Verify lanza SaltParseException si el 'hash' almacenado no es un bcrypt válido.
            // Para soportar ambos escenarios (texto plano vs bcrypt), intentamos bcrypt primero y luego hacemos fallback a texto.
            var stored = usuario.ContrasenaHash ?? string.Empty;
            var provided = dto.ContrasenaHash ?? string.Empty;

            try
            {
                if (!BCrypt.Net.BCrypt.Verify(provided, stored))
                    return Unauthorized(new { message = "Credenciales inválidas" });
            }
            catch (BCrypt.Net.SaltParseException)
            {
                // fallback a texto plano (solo para escenarios de BD vieja)
                if (!string.Equals(provided, stored, StringComparison.Ordinal))
                    return Unauthorized(new { message = "Credenciales inválidas" });
            }


            if (usuario.Rol == null)
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Usuario sin rol asociado (datos inconsistentes)" });

            if (usuario.Area == null)
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Usuario sin área asociada (datos inconsistentes)" });

            // 3. Generar token
            var token = GenerarToken(usuario);

            return Ok(new
            {
                token,
                usuario = new
                {
                    usuario.Id,
                    usuario.Nombre,
                    usuario.Correo,
                    usuario.Telefono,
                    usuario.Estado,
                    usuario.Area.NombreArea,
                    Rol = usuario.Rol.Nombre
                }
            });
        }

        private string GenerarToken(Usuario usuario)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Email, usuario.Correo),
                new Claim(ClaimTypes.Name, usuario.Nombre),
                new Claim(ClaimTypes.Role, usuario.Rol.Nombre)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expira = DateTime.UtcNow.AddMinutes(
                double.Parse(_config["Jwt:ExpiresInMinutes"]!));

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: expira,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}