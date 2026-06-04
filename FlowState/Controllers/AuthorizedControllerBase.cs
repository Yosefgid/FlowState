using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FlowState.Controllers
{
    public abstract class AuthorizedControllerBase : ControllerBase
    {
        protected int? GetLoggedInUserId()
        {
            //Get the UserId from the token, if automatic translation is off it will fallback to using "sub" to find UserId
            //returns null if sub does not exisit 
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                      ?? User.FindFirst("sub")?.Value;

            return int.TryParse(claim, out var id) ? id : null;
        }
    }
}
