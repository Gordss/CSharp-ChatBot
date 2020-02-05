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
using static WebAPITest.Controllers.Query;

namespace WebAPITest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CallController : ControllerBase
    {
        //GET: api/Call

        /// <summary>
        /// Gets JSON (responce from LUIS) sends the data to the ICB API
        /// </summary>
        /// <returns>A JSON object with data</returns>
        //[HttpGet]
        public async Task<ResponseData> GetFromApiAsync(RootObject data)
        {

            HttpClient client = new HttpClient();

            TopScoringIntent processedIntent = data.topScoringIntent;

            string actualQuery = string.Empty;
            string url = "https://i4sbprod-apimanagement.azure-api.net/analysis/";

            switch (processedIntent.intent)//compose the query
            {
                case "None":
                    return new ResponseData();
                case "MachineRequestData":
                    url += "Machine/Last";
                    List<Entity> dataInJSON3 = data.entities;
                    MachineRequestData currentObj3 = new MachineRequestData();
                    currentObj3.SensorID = dataInJSON3.Find(e => e.type == "MachineID").entity;
                    currentObj3.Type = dataInJSON3.Find(e => e.type == "MachineRequestType").resolution.values[0];
                    actualQuery = JsonConvert.SerializeObject(currentObj3);
                    break;
                default:
                    url += "KPI/Last";
                    List<Entity> dataInJSON1 = data.entities;
                    KPIRequestDataWithoutPart currentObj1 = new KPIRequestDataWithoutPart();
                    int buf;
                    currentObj1.KpiType = processedIntent.intent;
                    currentObj1.WorkOrder = dataInJSON1.Find(e => e.type == "KPIworkOrderID" && int.TryParse(e.entity, out buf)).entity;

                    //if (processedIntent.intent == "OrderRemainingWorkDeviation" ||
                    //    processedIntent.intent == "OrderEstimatedRemainigWork" ||
                    //    processedIntent.intent == "OrderActualRemainigWork")
                    //{
                    //    var temp = data.CompositeEntities.Find(e => e.parentType == "KPIrequestDataPart" && int.TryParse(e.value, out buf)).value;
                    //    if (temp != null)
                    //    {
                    //        return new string[] { "ErrorWithPart" };
                    //    }
                    //}


                    if (processedIntent.intent != "OrderRemainingWorkDeviation" &&
                        processedIntent.intent != "OrderEstimatedRemainigWork" &&
                        processedIntent.intent != "OrderActualRemainigWork")
                    {

                        string kpiOrderPart = data.CompositeEntities.Find(e => (e.parentType == "KPIrequestDataPart" && int.TryParse(e.value, out buf) )).value;
                        if (kpiOrderPart != null)
                        {
                            string serializedParent = JsonConvert.SerializeObject(currentObj1);
                            KPIRequestDataWithPart child = JsonConvert.DeserializeObject<KPIRequestDataWithPart>(serializedParent);
                            child.Part = kpiOrderPart;
                            actualQuery = JsonConvert.SerializeObject(child);
                            break;
                        }
                        else
                        {
                            //couldn't find part value
                            throw new NullReferenceException();
                        }
                    }

                    //KPIRequestData currentObj1 = new KPIRequestData();
                    //currentObj1.KpiType = "OTD";
                    //currentObj1.WorkOrder = "8383";
                    //currentObj1.part = "1";

                    actualQuery = JsonConvert.SerializeObject(currentObj1);
                    break;
            }

            //actualQuery = "{\"workOrder\":\"8383\",\"kpiType\":\"OTD\",\"part\":\"1\"}";

            //actualQuery = actualQuery.Trim('\\').Substring(1,actualQuery.Length-2);


            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "1f0563fd67ca436db10cc4bdef08aeea");
            HttpResponseMessage message = await client.PostAsync(url, new StringContent(actualQuery, Encoding.UTF8, "application/json"));

            //await Task.Delay(1000);

            var json = await message.Content.ReadAsStringAsync();

            var topIntent = JObject.Parse(json.Trim('"', '"').Trim('[', ']')).ToObject<ResponseData>();

            //string shit = "";
            //var json1 = JsonConvert.SerializeObject(json).ToString();
            //var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(json1);

            //var result = dict["Result"].Trim('"', '"').Trim('[', ']');
            //var dict2 = JsonConvert.DeserializeObject<Dictionary<string, string>>(result);

            //foreach (var kv in dict2)
            //{
            //    shit += kv;
            //}

            //string whole = result + shit;

            return topIntent;
        }

        /// <summary>
        /// Gets message from WPF and sends data to LUIS.ai and return json object
        /// </summary>
        /// <param name="utterance">the WPF user message</param>
        /// <returns>A JSON Object with data to make the query to send</returns>
        // GET: api/Call/5
        //[HttpGet("{id}", Name = "Get")]
        //[HttpGet]
        public async Task<IEnumerable<RootObject>> GetFromLuisAsync(string utterance)
        {

            utterance = "what is the otd for order 8383 in part 1? ";

            string key = "5685e7ac3ed241dc9d03f4d5ed712420";

            string endpoint = "https://westus.api.cognitive.microsoft.com/luis/v2.0/apps";

            string LUISAppID = "01784b20-3799-4171-881c-f42609e2d4aa";

            IEnumerable<string> luisResponce = await MakeRequest(key, endpoint, LUISAppID, utterance);

            JToken ents = JObject.Parse(luisResponce.ElementAt(0)).SelectToken("entities");
            string q = JObject.Parse(luisResponce.ElementAt(0)).SelectToken("query").ToString();
            JToken ints = JObject.Parse(luisResponce.ElementAt(0)).SelectToken("intents");
            TopScoringIntent topIntent = JObject.Parse(luisResponce.ElementAt(0)).SelectToken("topScoringIntent").ToObject<TopScoringIntent>();
            JToken compEnts = JObject.Parse(luisResponce.ElementAt(0)).SelectToken("compositeEntities");


            List<Entity> entities = new List<Entity>();
            List<Intent> intents = new List<Intent>();
            List<CompositeEntity> compositeEntities = new List<CompositeEntity>();

            foreach (var item in ents)
            {
                entities.Add(item.ToObject<Entity>());
            }
            foreach (var item in ints)
            {
                intents.Add(item.ToObject<Intent>());
            }
            if (compEnts != null)
            {
                foreach (var item in compEnts)
                {
                    compositeEntities.Add(item.ToObject<CompositeEntity>());
                }
            }

            RootObject wholeData = new RootObject
            {
                entities = entities,
                intents = intents,
                topScoringIntent = topIntent,
                query = q,
                CompositeEntities = compositeEntities
            };

            return new RootObject[] { wholeData };
        }


        static async Task<IEnumerable<string>> MakeRequest(string key, string endpoint, string appId, string utterance)
        {
            var client = new HttpClient();

            var endpointUri = String.Format("{0}/{1}?verbose=true&timezoneOffset=0&subscription-key={2}&q={3}", endpoint, appId, key, utterance);

            var response = await client.GetAsync(endpointUri);

            var strResponseContent = await response.Content.ReadAsStringAsync();

            return new string[] { strResponseContent.ToString() };
        }

        ///// <summary>
        ///// Process the final string and output it in the WPF
        ///// </summary>
        ///// <param name="data"></param>
        [HttpGet]
        public async Task<string> ProcessFinalStringAsync(string query)
        {
            query = "";//to come from the wpf

            var data = (await this.GetFromLuisAsync(query)).ElementAt(0);

            var response = await this.GetFromApiAsync(data); 

            if (response.ToString() == "Error")
            {
                return new string
                (
                "I could not figure out the meaning of your query or something is wrong with the data u are trying to access. If you are trying to access a machine, please make sure it's ID starts with a capital M. Please try again with a valid query."
                );
            }
            if (response.ToString() == "ErrorWithPart")
            {
                return new string
                (
                    "This entity does not require part parameter."
                );
            }

            //TODO proccess the final string to return to the wpf, meaning the plain text to return

            //string[] resp = {
            //                "ScrapPercentage",
            //                "M123",
            //                "12"
            //                };

            string[] resp = {
                            "OTD",
                            "8383",
                            "12",
                            "33"
                            };

            string result = ("The " + resp[0] + " of ");

            if (resp[1][0] == 'M')
            {
                result += "machine " + resp[1];
            }
            else
            {
                result += "order " + resp[1];
                if (resp.Length == 4)
                {
                    result += " of part " + resp[3];
                }
            }

            result += " is: " + resp[2];

            return new string(result);
        }
    }
}
