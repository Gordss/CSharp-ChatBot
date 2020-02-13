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
using static WebAPITest.Controllers.Query;
using WebAPITest.Settings;

namespace WebAPITest.Services
{
    public class RecognitionService
    {
        //GET: api/Call

        /// <summary>
        /// Gets query as a string (response from LUIS) and intent type then sends the data to the ICB API
        /// </summary>
        /// <returns>ResponceData object</returns>
        //[HttpGet]
        private async Task<ResponseData> GetFromApiAsync(string actualQuery, string intent)
        {

            HttpClient client = new HttpClient();

            //string url = "https://i4sbprod-apimanagement.azure-api.net/analysis/";
            string url = Credentials.API_URL;
            if (intent == "MachineRequestData")
            {
                url += "Machine/Last";
            }
            else
            {
                url += "KPI/Last";
            }

            //client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "1f0563fd67ca436db10cc4bdef08aeea");
            client.DefaultRequestHeaders.Add(Credentials.API_HEADER, Credentials.API_ID);
            HttpResponseMessage message = await client.PostAsync(url, new StringContent(actualQuery, Encoding.UTF8, "application/json"));

            var json = await message.Content.ReadAsStringAsync();

            if (json == "[]")
            {
                throw new Exception("No existing data for the query!");
            }

            var topIntent = JObject.Parse(json.Trim('"', '"').Trim('[', ']')).ToObject<ResponseData>();

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
        private async Task<IEnumerable<RootObject>> GetFromLuisAsync(string utterance)
        {

            //string key = "5685e7ac3ed241dc9d03f4d5ed712420";
            string key = Credentials.LUIS_KEY;

            //string endpoint = "https://westus.api.cognitive.microsoft.com/luis/v2.0/apps";
            string endpoint = Credentials.LUIS_URL;

            //string LUISAppID = "01784b20-3799-4171-881c-f42609e2d4aa";
            string LUISAppID = Credentials.LUIS_ID;

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

            if (topIntent.intent != "MachineRequestData")
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

        /// <summary>
        /// Gets JSON (response data from LUIS) object and returns query as a string
        /// </summary>
        /// <param name="data">the response data from LUIS</param>
        /// <returns></returns>
        private string ConstructQueryHelper(RootObject data)
        {
            TopScoringIntent processedIntent = data.topScoringIntent;
            string actualQuery = string.Empty;
            switch (processedIntent.intent)//compose the query
            {
                case "None":
                    throw new Exception("Please paraphrase your question, couldn't understand what you meant!");

                case "MachineRequestData":

                    List<Entity> machineEntities = data.entities;
                    MachineRequestData machineRequest = new MachineRequestData();
                    machineRequest.SensorID = machineEntities.Find(e => e.type == "MachineID").entity;
                    machineRequest.Type = machineEntities.Find(e => e.type == "MachineRequestType").resolution.values[0];
                    actualQuery = JsonConvert.SerializeObject(machineRequest);
                    break;
                default:
                    List<Entity> kpiEntities = data.entities;
                    KPIRequestDataWithoutPart kpiRequestWithoutPart = new KPIRequestDataWithoutPart();
                    int buf;
                    kpiRequestWithoutPart.KpiType = processedIntent.intent;
                    kpiRequestWithoutPart.WorkOrder = kpiEntities.Find(e => e.type == "KPIworkOrderID" && int.TryParse(e.entity, out buf)).entity;

                    //get part entity
                    CompositeEntity kpiOrderPart = data.CompositeEntities.Find(e => (e.parentType == "KPIrequestDataPart" && int.TryParse(e.value, out buf)));

                    //filter request types without part
                    if (processedIntent.intent != "OrderRemainingWorkDeviation" &&
                        processedIntent.intent != "OrderEstimatedRemainigWork" &&
                        processedIntent.intent != "OrderActualRemainigWork" &&
                        kpiOrderPart != null)
                    {

                        string kpiOrderPartValue = kpiOrderPart.value;
                        if (kpiOrderPart != null)
                        {
                            string serializedKPIWithoutPart = JsonConvert.SerializeObject(kpiRequestWithoutPart);
                            KPIRequestDataWithPart kpiRequestWithPart = JsonConvert.DeserializeObject<KPIRequestDataWithPart>(serializedKPIWithoutPart);
                            kpiRequestWithPart.Part = kpiOrderPartValue;
                            actualQuery = JsonConvert.SerializeObject(kpiRequestWithPart);
                            break;
                        }
                        else
                        {
                            //couldn't find part value
                            throw new NullReferenceException("Couldn't find part value");
                        }
                    }

                    actualQuery = JsonConvert.SerializeObject(kpiRequestWithoutPart);
                    break;
            }
            return actualQuery;
        }

        private static async Task<IEnumerable<string>> MakeRequest(string key, string endpoint, string appId, string utterance)
        {
            var client = new HttpClient();

            var endpointUri = String.Format("{0}/{1}?verbose=true&timezoneOffset=0&subscription-key={2}&q={3}", endpoint, appId, key, utterance);

            var response = await client.GetAsync(endpointUri);

            var strResponseContent = await response.Content.ReadAsStringAsync();

            return new string[] { strResponseContent.ToString() };
        }

        /// <summary>
        /// Gets user request, sends it to LUIS, gets data from ICB API and returns the final answer
        /// </summary>
        /// <param name="userRequestMessage">user message</param>
        // <returns>API message</returns>
        public async Task<string> ProcessText(string userRequestMessage)
        {

            RootObject luisResponseData;

            try
            {
                luisResponseData = (await this.GetFromLuisAsync(userRequestMessage)).ElementAt(0);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            string queryForAPI;

            try
            {
                queryForAPI = ConstructQueryHelper(luisResponseData);
            }
            catch (Exception ex)
            {
                if (ex is NullReferenceException)
                {
                    return "Invalid query!";
                }
                return ex.Message;
            }

            TopScoringIntent processedIntent = luisResponseData.topScoringIntent;
            ResponseData responseFromAPI;

            try
            {
                responseFromAPI = await this.GetFromApiAsync(queryForAPI, processedIntent.intent);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            string entity;
            string entityID;
            string partNumber = string.Empty;
            string responseValue = responseFromAPI.value;

            if (processedIntent.intent == "MachineRequestData")
            {
                var machineRequestObj = JsonConvert.DeserializeObject<MachineRequestData>(queryForAPI);
                entity = machineRequestObj.Type;
                entityID = machineRequestObj.SensorID;
            }
            else
            {
                var kpiRequestObj = JsonConvert.DeserializeObject<KPIRequestDataWithPart>(queryForAPI);
                entity = kpiRequestObj.KpiType;
                entityID = kpiRequestObj.WorkOrder;

                if (kpiRequestObj.Part != string.Empty && kpiRequestObj.Part != null)
                {
                    partNumber = kpiRequestObj.Part;
                }

            }
            string result = string.Format("The {0} of {1}{2} is {3}.",
                                          entity,
                                          entityID,
                                          (partNumber != string.Empty) ? (" of part " + partNumber) : "",
                                          responseValue
                                          );

            return result;
        }
    }
}
