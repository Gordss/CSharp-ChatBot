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

namespace WebAPITest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CallController : ControllerBase
    {
        //GET: api/Call

        /// <summary>
        /// Get the data from the ICB API
        /// </summary>
        /// <returns>A JSON object with data</returns>
        //[HttpGet]
        [HttpGet("{utterance}", Name = "Get")]
        public async Task<IEnumerable<string>> GetFromApiAsync(string utterance)
        {

            HttpClient client = new HttpClient();

            var data = await this.GetFromLuisAsync(utterance);
            //use the data from LUIS to make the query and get the respective data from the ICB API
            //TODO needs to be finished

            var intent = JsonConvert.DeserializeObject<Dictionary<string, string>>(data.ToString());

            string processedIntent = intent["prediction"];

            string actualQuery = string.Empty;

            switch (processedIntent)
            {
                case "KPIRequestData":
                    //compose the respective query eg KPIRequestData
                    var dataInJSON1 = JsonConvert.DeserializeObject<Dictionary<string,string>>(intent["entities"]);
                    KPIRequestData currentObj1 = new KPIRequestData();
                    currentObj1.kpiType = dataInJSON1["kpiType"];
                    currentObj1.workOrder = dataInJSON1["workOrder"];
                    currentObj1.part = dataInJSON1["part"];
                    actualQuery = JsonConvert.SerializeObject(currentObj1);
                    break;
                case "KPIRequestDataInRange":
                    //compose the respective query eg KPIRequestDataInRange
                    var dataInJSON2 = JsonConvert.DeserializeObject<Dictionary<string, string>>(intent["entities"]);
                    KPIRequestDataInRange currentObj2 = new KPIRequestDataInRange();
                    currentObj2.kpiType = dataInJSON2["kpiType"];
                    currentObj2.workOrder = dataInJSON2["workOrder"];
                    currentObj2.part = dataInJSON2["part"];
                    currentObj2.before = DateTime.Parse(dataInJSON2["before"]);
                    currentObj2.after = DateTime.Parse(dataInJSON2["after"]);
                    currentObj2.count = Int32.Parse(dataInJSON2["count"]);
                    actualQuery = JsonConvert.SerializeObject(currentObj2);
                    break;
                case "MachineRequestData":
                    //compose the respective query eg MachineRequestData
                    var dataInJSON3 = JsonConvert.DeserializeObject<Dictionary<string, string>>(intent["entities"]);
                    MachineRequestData currentObj3 = new MachineRequestData();
                    currentObj3.sensorId = dataInJSON3["Machine"];
                    currentObj3.type = dataInJSON3["type"];
                    actualQuery = JsonConvert.SerializeObject(currentObj3);
                    break;
                case "MachineRequestDataInRange":
                    //compose the respective query eg MachineRequestDataInRange
                    var dataInJSON4 = JsonConvert.DeserializeObject<Dictionary<string, string>>(intent["entities"]);
                    MachineRequestDataInRange currentObj4 = new MachineRequestDataInRange();
                    currentObj4.sensorId = dataInJSON4["Machine"];
                    currentObj4.type = dataInJSON4["type"];
                    currentObj4.before = DateTime.Parse(dataInJSON4["before"]);
                    currentObj4.after = DateTime.Parse(dataInJSON4["after"]);
                    currentObj4.count = Int32.Parse(dataInJSON4["count"]);
                    actualQuery = JsonConvert.SerializeObject(currentObj4);
                    break;
                default://When the intent isnt recognised to return an error message to the WPF
                    //TODO
                    ProcessFinalStringAsync(new string[] { "Error" });
                    break;
            }

            string query = "{\"workOrder\":\"8383\",\"kpiType\":\"OTD\",\"part\":\"1\"}";

            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "1f0563fd67ca436db10cc4bdef08aeea");
            HttpResponseMessage message = await client.PostAsync("https://i4sbprod-apimanagement.azure-api.net/analysis/KPI/Last", new StringContent(query, Encoding.UTF8, "application/json"));

            //await Task.Delay(1000);


            var json = message.Content.ReadAsStringAsync();
            string shit = "";
            var json1 = JsonConvert.SerializeObject(json).ToString();
            var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(json1);

            var result = dict["Result"].Trim('"', '"').Trim('[', ']');
            var dict2 = JsonConvert.DeserializeObject<Dictionary<string, string>>(result);

            foreach (var kv in dict2)
            {
                shit += kv;
            }

            //string whole = result + shit;

            return new string[] { shit };

        }
        

        /// <summary>
        /// A method to get the data from LUIS.ai
        /// </summary>
        /// <param name="id"></param>
        /// <returns>A JSON Object with data to make the query to send</returns>
        // GET: api/Call/5
        //[HttpGet("{id}", Name = "Get")]
        public async Task<IEnumerable<string>> GetFromLuisAsync(string incomingStr)
        {
            // YOUR-KEY: for example, the starter key
            var key = "5685e7ac3ed241dc9d03f4d5ed712420";

            // YOUR-ENDPOINT: example is westus2.api.cognitive.microsoft.com
            var endpoint = "https://westus.api.cognitive.microsoft.com/luis/api/v2.0";

            // //public sample app
            var appId = "01784b20-3799-4171-881c-f42609e2d4aa";

            var utterance = incomingStr;

            var result = await MakeRequest(key, endpoint, appId, utterance);

            return new string[] { result.ToString() };
        }



        static async Task<IEnumerable<string>> MakeRequest(string key, string endpoint, string appId, string utterance)
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // The request header contains your subscription key
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", key);

            // The "q" parameter contains the utterance to send to LUIS
            queryString["query"] = utterance;

            // These optional request parameters are set to their default values
            queryString["verbose"] = "true";
            queryString["show-all-intents"] = "true";
            queryString["staging"] = "true";
            queryString["timezoneOffset"] = "0";

            var endpointUri = String.Format("https://{0}/luis/prediction/v3.0/apps/{1}/slots/production/predict?query={2}", endpoint, appId, queryString);

            var response = await client.GetAsync(endpointUri);

            var strResponseContent = await response.Content.ReadAsStringAsync();

            // Return the JSON result from LUIS
            return new string[] { strResponseContent.ToString() };
        }

        /// <summary>
        /// Process the final string and output it in the WPF
        /// </summary>
        /// <param name="data"></param>
        public async Task<string> ProcessFinalStringAsync(string[] data)
        {
            await this.GetFromApiAsync(string.Empty);

            string result = "";

            if (data.ToString() == "Error")
            {
                return "ErrorMessage";
                //TODO
                //Needs to return a proper error message
            }

            return result;
        }


    }
}
