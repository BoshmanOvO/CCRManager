using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using CCRManager.Models;

namespace CCRManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JwtController : ControllerBase
    {
        private readonly JwtSettings _jwtSettings;
        public JwtController(JwtSettings jwtSettings) => _jwtSettings = jwtSettings;

        [HttpPost("token")]
        public IActionResult GenerateToken()
        {
            var claims = new[]
            {
                // A claim is a piece of information inside the JWT that defines who the user is.
                new Claim(JwtRegisteredClaimNames.Sub, "testuser"), // Subject claim, set to "testuser" (identifies the user)
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // Unique token identifier (prevents token reuse).
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // creating the token
            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.Now.AddSeconds(_jwtSettings.ExpiryMinutes),
                signingCredentials: creds
            );
            return Ok(new JwtSecurityTokenHandler().WriteToken(token));
        }
    }
}
