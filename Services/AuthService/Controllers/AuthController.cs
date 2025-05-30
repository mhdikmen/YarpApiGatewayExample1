using BuildingBlocks.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;

        public AuthController(ILogger<AuthController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        [HttpPost("token")]
        public IActionResult Get([FromBody] UserCredentials creds, [FromHeader(Name = "X-Target-Audience")] string? audience)
        {
            if (string.IsNullOrEmpty(audience))
            {
                return BadRequest("X-Target-Audience header is required.");
            }
            if (creds.Username != "user1" || creds.Password != "pass") return Unauthorized();
            var token = GenerateJwt(creds.Username, audience);
            return Ok(new
            {
                access_token = token,
            });
        }

        private string GenerateJwt(string username, string audience)
        {
            string issuer = _configuration["Jwt:Issuer"] ?? throw new RequiredConfigurationException("Jwt:Issuer");
            string configKey = _configuration["Jwt:Key"] ?? throw new RequiredConfigurationException("Jwt:Key");

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class UserCredentials
    {
        public string Username { get; set; } = default!;
        public string Password { get; set; } = default!;
    }
}
