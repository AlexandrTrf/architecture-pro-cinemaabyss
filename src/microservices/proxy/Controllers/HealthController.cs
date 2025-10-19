using Microsoft.AspNetCore.Mvc;

namespace proxy.Controllers;

[ApiController]
[Route("api/proxy/health")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult GetHealth() => Ok(new {status = true});
}