using FlowState.Models;
using FlowState.Models.DTOs;
using FlowState.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace FlowState.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : AuthorizedControllerBase
    {
        private readonly IUserServices _userServices;
        public UsersController(IUserServices userServices)
        {
            _userServices = userServices;
        }
        [HttpGet]
        public IActionResult GetAllUsers()
        {
            //Big Issue, this will provide all users 
            //TODO: replace with role-based admin check once the role system exisit
            return Forbid();
        }

        [HttpGet("{id}")]
        public IActionResult GetUserById(int id)
        {

            var userID = GetLoggedInUserId();
            if(userID == null) return Unauthorized();
            if (userID != id) return Forbid();

            var user = _userServices.GetUserById(id);
            if (user == null) return NotFound();
            return Ok(user);

        }

        [HttpPut("{id}")]
        public IActionResult UpdateUser(int id, [FromBody] UpdateUserDto dto)
        {
            var userId = GetLoggedInUserId();
            if (userId == null) return Unauthorized();
            if (userId != id) return Forbid();

            if (string.IsNullOrWhiteSpace(dto.Email))
                return BadRequest("Email is required.");

            var existing = _userServices.GetUserById(id);
            if (existing == null) return NotFound();

            existing.Email = dto.Email;
            var updated = _userServices.UpdateUser(id, existing);
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            var userId = GetLoggedInUserId();
            if (userId == null) return Unauthorized();
            if (userId != id) return Forbid();

            var deleted = _userServices.DeleteUser(id);
            if (!deleted) return NotFound();
            return NoContent();
        }

        [HttpPatch("{id}/username")]
        public IActionResult ChangeUsername(int id, [FromBody] ChangeUsernameDto dto)
        {
            var userId = GetLoggedInUserId();
            if (userId == null) return Unauthorized();
            if (userId != id) return Forbid();

            if (string.IsNullOrWhiteSpace(dto.NewUsername))
                return BadRequest("New username is required.");

            var updated = _userServices.ChangeUsername(id, dto.NewUsername);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

       


    }
}
