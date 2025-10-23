using Microsoft.AspNetCore.Mvc;

namespace events.Controllers;

[ApiController]
[Route("api/events/health")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult GetHealth() => Ok(new {status = true});
}