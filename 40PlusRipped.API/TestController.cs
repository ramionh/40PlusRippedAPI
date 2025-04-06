using Microsoft.AspNetCore.Mvc;

namespace _40PlusRipped.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { message = "40PlusRipped API is working!" });
        }
    }
}