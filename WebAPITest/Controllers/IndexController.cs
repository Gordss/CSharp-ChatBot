using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPITest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IndexController : ControllerBase
    {
        // GET: api/Index
        [HttpGet]
        public IActionResult Get()
        {
            return Content("This is dummy page :)");
        }
    }
}
