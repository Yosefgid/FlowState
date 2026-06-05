using FlowState.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowState.Controllers
{
    [Authorize]
    [Route("api/google-tasks")]
    [ApiController]
    public class GoogleToDoTaskController : AuthorizedControllerBase
    {
        private readonly IGoogleService _googleService;

        public GoogleToDoTaskController(IGoogleService googleService)
        {
            _googleService = googleService;
        }

        [HttpGet("status")]
        public async Task<IActionResult> Status()
        {
            var userId = GetLoggedInUserId();

            if (userId == null)
            {
                return Unauthorized();
            }

            var isConnected = await _googleService
                .IsGoogleCalendarConnectedAsync(userId.Value.ToString());

            return Ok(new { isConnected });
        }

        [HttpGet("auth-url")]
        public IActionResult GetAuthUrl()
        {
            var userId = GetLoggedInUserId();

            if (userId == null)
            {
                return Unauthorized();
            }

            var authUrl = _googleService.GetGoogleAuthUrl(userId.Value.ToString());

            return Ok(new { authUrl });
        }

        [HttpGet("callback")]
        [AllowAnonymous]
        public async Task<IActionResult> Callback(
            [FromQuery] string code,
            [FromQuery] string state)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return BadRequest("Missing Google authorization code.");
            }

            if (string.IsNullOrWhiteSpace(state))
            {
                return BadRequest("Missing user state.");
            }

            await _googleService.ConnectGoogleCalendarAsync(code, state);

            return Redirect("http://localhost:5270/calendar");
        }

        [HttpPost("import")]
        public async Task<IActionResult> ImportGoogleEvents()
        {
            var userId = GetLoggedInUserId();

            if (userId == null)
            {
                return Unauthorized();
            }

            var importedTasks = await _googleService
                .ImportGoogleCalendarEventsAsync(userId.Value.ToString());

            return Ok(importedTasks);
        }
    }
}