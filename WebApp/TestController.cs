using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApp
{
    [Route("api/[controller]")]
    public class TestController : Controller
    {
        // GET: api/<controller>
        [HttpGet]
        public async Task<IEnumerable<string>> GetAsync()
        {
            /*
            string query = "workOrder";
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "1f0563fd67ca436db10cc4bdef08aeea");
            HttpResponseMessage message = await client.PostAsync("https://i4sbprod-apimanagement.azure-api.net/analysis/KPI/Last", new StringContent(query, Encoding.UTF8, "application/json"));
            */
            return new string[] { "value1", "value2" };
        }
    }
}
