using FlowState.Services;
using Microsoft.AspNetCore.Mvc;

namespace FlowState.Controllers
{
    [Route("api/google-tasks")]
    [ApiController]
    public class GoogleToDoTaskController : ControllerBase
    {
        private readonly IGoogleService _googleService;

        public GoogleToDoTaskController(IGoogleService googleService)
        {
            _googleService = googleService;
        }

        [HttpGet("connect")]
        public IActionResult Connect()
        {
            var userId = GetLocalUserId();

            var authUrl = _googleService.GetGoogleAuthUrl(userId);

            return Redirect(authUrl);
        }

        [HttpGet("callback")]
        public async Task<IActionResult> Callback([FromQuery] string code, [FromQuery] string state)
        {
            await _googleService.ConnectGoogleCalendarAsync(code, state);

            return Ok("Google Calendar connected successfully.");
        }

        [HttpPost("import")]
        public async Task<IActionResult> ImportGoogleEvents()
        {
            var userId = GetLocalUserId();

            var importedTasks = await _googleService.ImportGoogleCalendarEventsAsync(userId);

            return Ok(importedTasks);
        }

        private string GetLocalUserId()
        {
            return User.Identity?.Name ?? "local-user";
        }
    }
}