using AspNetBlog.Attributes;
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
        [ApiKey]
        public IActionResult Get()
        {
            return Ok();
        }


    }
}
