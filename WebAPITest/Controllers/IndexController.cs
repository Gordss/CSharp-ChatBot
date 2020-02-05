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
        public IEnumerable<string> Get()
        {
            return new string[] { "boza", "val1" };
        }

        // GET: api/Index/5
        [HttpGet("{utterance}", Name = "Get")]
        public string Get(string utterance)
        {
            return "value";
        }

        // POST: api/Index
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Index/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
