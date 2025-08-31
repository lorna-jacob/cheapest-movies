using Microsoft.AspNetCore.Mvc;

namespace Webjet.Api.Controllers
{
    [ApiController]
    public class HealthCheckController : ControllerBase
    {
        [HttpGet("ping")]
        public IActionResult Live() => Ok("pong");
    }
}