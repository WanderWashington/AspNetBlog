using Microsoft.AspNetCore.Mvc;

//Health Check
namespace AspNetBlog.Controllers
{
    [ApiController]
    [Route("")]
    public class HomeController: ControllerBase
    {   
        //HealthCheck
        [HttpGet("")]
        public IActionResult Get()
        {
            return Ok();
        }


    }
}
