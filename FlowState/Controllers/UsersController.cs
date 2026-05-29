using FlowState.Models;
using FlowState.Models.DTOs;
using FlowState.Services;
using Microsoft.AspNetCore.Mvc;

namespace FlowState.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserServices _userServices;
        public UsersController(IUserServices userServices)
        {
            _userServices = userServices;
        }
        [HttpGet]
        public IActionResult GetAllUsers()
        {
            try
            {
                var users = _userServices.GetAllUser();
                return Ok(users);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while retrieving users.");
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetUserById(int id)
        {

            var user = _userServices.GetUserById(id);

            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpPost]
        public IActionResult CreateUser([FromBody] CreateUserDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Username))
                return BadRequest("Username is required.");

            if (string.IsNullOrWhiteSpace(dto.Email))
                return BadRequest("Email is required.");

            if (string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest("Password is required.");

            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email
            };

            var created = _userServices.AddUser(user, dto.Password);

            if (created == null)
                return BadRequest("Could not create user. Username or email may already be taken.");

            return CreatedAtAction(nameof(GetUserById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateUser(int id, [FromBody] UpdateUserDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email))
                return BadRequest("Email is required.");

            var existing = _userServices.GetUserById(id);

            if (existing == null)
                return NotFound();

            existing.Email = dto.Email;

            var updated = _userServices.UpdateUser(id, existing);

            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            var deleted = _userServices.DeleteUser(id);

            if (!deleted)
                return NotFound();

            return NoContent();
        }

        [HttpPatch("{id}/username")]
        public IActionResult ChangeUsername(int id, [FromBody] ChangeUsernameDto dto)
        {

            if (string.IsNullOrWhiteSpace(dto.NewUsername))
                return BadRequest("New username is required.");

            var updated = _userServices.ChangeUsername(id, dto.NewUsername);

            if (updated == null)
                return NotFound();

            return Ok(updated);
        }


    }
}
