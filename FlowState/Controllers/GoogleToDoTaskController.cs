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

        [HttpGet("status")]
        public async Task<IActionResult> Status()
        {
            var userId = GetLocalUserId();

            var isConnected = await _googleService.IsGoogleCalendarConnectedAsync(userId);

            return Ok(new { isConnected });
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

            return Redirect("http://localhost:5270/calendar");
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