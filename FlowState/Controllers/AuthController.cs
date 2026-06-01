using FlowState.Models;
using FlowState.Models.DTOs.Auth;
using FlowState.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;


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
            if (string.IsNullOrWhiteSpace(dto.Username))
            {
                return BadRequest("Username is required");
            };
            if (string.IsNullOrWhiteSpace(dto.Email) || !new EmailAddressAttribute().IsValid(dto.Email))
            {
                return BadRequest("A valid email is required");
            };
            if(string.IsNullOrWhiteSpace(dto.Password) || dto.Password.Length < 8 || !Regex.IsMatch(dto.Password, @"^(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).+$"))
            {
                return BadRequest("Password must be at 8 or more characters and contain one uppercase letter, one number and one special character.");
            }
            if(dto.Password != dto.ConfirmPassword)
            {
                return BadRequest("Password do not match try again ");
            }
            var user = _authServices.Register(dto.Username, dto.Email, dto.Password);
            if (user == null)
            {
                return Conflict(new { message = "Email or username is already in use." });
            }
            //instead of returning the uri retun an empty string
            //allows us to isolate user management routes 
            return Created(string.Empty, BuildAuthResponse(user));
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
