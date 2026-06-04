using FlowState.Models;
using FlowState.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlowState.Controllers
{
    [Route("api/sessions")]
    [ApiController]
    public class SessionController : Controller
    {
        private ISessionService _sessionService;

        public SessionController(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        // GET session/user/5
        [HttpGet("user/{userId}")]
        public ActionResult<List<Session>> GetSessionsByUser(int userId)
        {
            return Ok(_sessionService.GetSessionsByUser(userId));
        }

        // GET session/5
        [HttpGet("{id}")]
        public ActionResult<Session> GetSession(int id)
        {
            var session = _sessionService.GetSession(id);

            if (session == null)
                return NotFound();

            return Ok(session);
        }

        // POST session/user/5
        [HttpPost("user/{userId}")]
        public ActionResult<Session> AddSession(int userId, [FromBody] Session session)
        {
            var createdSession = _sessionService.AddSession(userId, session);

            return CreatedAtAction(
                nameof(GetSession),
                new { id = createdSession.Id },
                createdSession);
        }

        // PUT session/5
        [HttpPut("{id}")]
        public ActionResult<Session> UpdateSession(int id, [FromBody] Session updatedSession)
        {
            var session = _sessionService.UpdateSession(id, updatedSession);

            if (session == null)
                return NotFound();

            return Ok(session);
        }

        // DELETE session/5
        [HttpDelete("{id}")]
        public IActionResult DeleteSession(int id)
        {
            var deleted = _sessionService.DeleteSession(id);

            if (!deleted)
                return NotFound();

            return NoContent();
        }

        [HttpPost("{sessionId}/invite")]
        public ActionResult<string> CreateInvite(int sessionId)
        {
            var invite = new SessionInvite(sessionId);
           

            

            return Ok($"https://yourapp.com/invite/{invite.Token}");
        }

        //// POST session/5/user/10
        //[HttpPost("{sessionId}/user/{userId}")]
        //public ActionResult<SessionUser> AddSessionUser(int sessionId, int userId)
        //{
        //    var sessionUser = _sessionService.AddSessionUser(sessionId, userId);

        //    return Ok(sessionUser);
        //}

        // GET session-user/5
        [HttpGet("session-user/{sessionUserId}")]
        public ActionResult<SessionUser> GetSessionUser(int sessionUserId)
        {
            var sessionUser = _sessionService.GetSessionUser(sessionUserId);

            if (sessionUser == null)
                return NotFound();

            return Ok(sessionUser);
        }

        // GET session/5/users
        [HttpGet("{sessionId}/users")]
        public ActionResult<List<SessionUser>> GetSessionUsersBySession(int sessionId)
        {
            var sessionUsers = _sessionService.GetSessionUsersBySession(sessionId);

            return Ok(sessionUsers);
        }

        // DELETE session-user/5
        [HttpDelete("{userId}/session-user/{sessionId}")]
        public IActionResult DeleteSessionUser(int userId , int sessionId)
        {
            var deleted = _sessionService.DeleteSessionUser(userId,sessionId);

            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}

