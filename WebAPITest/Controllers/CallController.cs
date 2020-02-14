using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebAPITest.Models;
using static WebAPITest.Models.Query;
using WebAPITest.Services;


namespace WebAPITest.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class CallController : ControllerBase
    {
        /// <summary>
        /// Gets user request, sends it to LUIS, gets data from ICB API and returns the final answer
        /// </summary>
        /// <param name="userRequestMessage">user message</param>
        // <returns>API message</returns>
        [HttpGet("{userRequestMessage}", Name = "GetAnswer")]
        public async Task<IActionResult> ProcessFinalStringAsync(string userRequestMessage)
        {
            RecognitionService service = new RecognitionService();
            string userResponse = await service.ProcessText(userRequestMessage);
            return Content(userResponse);
        }
    }
}
