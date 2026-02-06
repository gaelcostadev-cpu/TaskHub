using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AuthService.Controllers
{

    [Authorize]
    [ApiController]
    public abstract class BaseAuthController : ControllerBase
    {
        protected Guid UserId =>
            Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        protected string Email =>
            User.FindFirstValue(ClaimTypes.Email)!;

        protected string Username =>
            User.FindFirstValue(ClaimTypes.Name)!;
    }
}
