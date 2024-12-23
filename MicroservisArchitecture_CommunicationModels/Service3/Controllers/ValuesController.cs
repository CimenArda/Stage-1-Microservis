using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Service3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            await Task.Delay(8000);
            return Ok(123);
        }
    }
}
