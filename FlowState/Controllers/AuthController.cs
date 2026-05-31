using FlowState.Models;
using FlowState.Models.DTOs.Auth;
using FlowState.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace FlowState.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthServices _authServices;
        private readonly IConfiguration _config;

        public AuthController(IAuthServices authService, IConfiguration config)
        {
            _authServices = authService;
            _config = config;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterDto dto)
        {
            var user = _authServices.Register(dto.Username, dto.Email, dto.Password);
            if (user == null)
            {
                return Conflict(new { message = "Email or username is already in use." });
            }

            return Ok(BuildAuthResponse(user));
        }
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto dto)
        {
            var user = _authServices.Login(dto.Email, dto.Password);
            if (user == null) return Unauthorized();
            return Ok(BuildAuthResponse(user));
        }

        private AuthResponseDto BuildAuthResponse(User user)
        {
            return new AuthResponseDto
            {
                Token = GenerateJwt(user),
                Username = user.Username,
                Email = user.Email
            };
         
        }
        private string GenerateJwt(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("username",                    user.Username),
                new Claim(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString())

            };
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
