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
using static WebAPITest.Models.Query.LuisResponse;
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
        private async Task<KPIApiResponse> GetFromApiAsync(string actualQuery, string intent)
        {

            HttpClient client = new HttpClient();

            string url = Credentials.API_URL;
            if (intent == "MachineRequestData")
            {
                url += "Machine/Last";
            }
            else
            {
                url += "KPI/Last";
            }

            client.DefaultRequestHeaders.Add(Credentials.API_HEADER, Credentials.API_ID);
            HttpResponseMessage message = await client.PostAsync(url, new StringContent(actualQuery, Encoding.UTF8, "application/json"));

            if (!message.IsSuccessStatusCode)
            {
                throw new Exception($"Request from ICB API failed with status code {message.StatusCode}");
            }

            string jsonResponce = await message.Content.ReadAsStringAsync();

            if (jsonResponce == "[]")
            {
                throw new Exception("No existing data for the query!");
            }

            KPIApiResponse topIntent = JObject.Parse(jsonResponce.Trim('"', '"').Trim('[', ']')).ToObject<KPIApiResponse>();

            return topIntent;
        }

        /// <summary>
        /// Gets message from WPF and sends data to LUIS.ai and return json object
        /// </summary>
        /// <param name="utterance">the WPF user message</param>
        /// <returns>A JSON Object with data to make the query to send</returns>
        private async Task<LuisResponse> GetFromLuisAsync(string utterance)
        {
            string key = Credentials.LUIS_KEY;
            string endpoint = Credentials.LUIS_URL;
            string LUISAppID = Credentials.LUIS_ID;

            string luisResponse = await MakeRequest(key, endpoint, LUISAppID, utterance);
            LuisResponse wholeData = JsonConvert.DeserializeObject<LuisResponse>(luisResponse);

            return wholeData;
        }

        /// <summary>
        /// Gets JSON (response data from LUIS) object and returns query as a string
        /// </summary>
        /// <param name="data">the response data from LUIS</param>
        /// <returns></returns>
        private string ConstructQueryHelper(LuisResponse data)
        {
            TopScoringIntent processedIntent = data.TopScoringIntent;
            string actualQuery = string.Empty;
            switch (processedIntent.Intent)//compose the query
            {
                case "None":
                    throw new Exception("Please paraphrase your question, couldn't understand what you meant!");

                case "MachineRequestData":

                    List<Entity> machineEntities = data.Entities;
                    MachineRequestData machineRequest = new MachineRequestData();
                    machineRequest.SensorID = machineEntities.Find(e => e.Type == "MachineID").entity;
                    machineRequest.Type = machineEntities.Find(e => e.Type == "MachineRequestType").resolution.values[0];
                    actualQuery = JsonConvert.SerializeObject(machineRequest);
                    break;
                default:
                    List<Entity> kpiEntities = data.Entities;
                    KPIRequestDataWithoutPart kpiRequestWithoutPart = new KPIRequestDataWithoutPart();
                    int buf;
                    kpiRequestWithoutPart.KpiType = processedIntent.Intent;
                    kpiRequestWithoutPart.WorkOrder = kpiEntities.Find(e => e.Type == "KPIworkOrderID" && int.TryParse(e.entity, out buf)).entity;

                    //get part entity
                    CompositeEntity kpiOrderPart = data.CompositeEntities.Find(e => (e.ParentType == "KPIrequestDataPart" && int.TryParse(e.Value, out buf)));

                    //filter request types without part
                    if (processedIntent.nonOrderQuery() && kpiOrderPart != null)
                    {
                        string kpiOrderPartValue = kpiOrderPart.Value;
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

        private async Task<string> MakeRequest(string key, string endpoint, string appId, string utterance)
        {
            HttpClient client = new HttpClient();

            string endpointUri = String.Format("{0}/{1}?verbose=true&timezoneOffset=0&subscription-key={2}&q={3}", endpoint, appId, key, utterance);

            HttpResponseMessage response = await client.GetAsync(endpointUri);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Request from LUIS failed with status code {response.StatusCode}");
            }

            string strResponseContent = await response.Content.ReadAsStringAsync();

            return strResponseContent;
        }

        /// <summary>
        /// Gets user request, sends it to LUIS, gets data from ICB API and returns the final answer
        /// </summary>
        /// <param name="userRequestMessage">user message</param>
        // <returns>API response message</returns>
        public async Task<string> ProcessText(string userRequestMessage)
        {

            LuisResponse luisResponseData;
            TopScoringIntent processedIntent = new TopScoringIntent();
            KPIApiResponse responseFromAPI = new KPIApiResponse();
            string queryForAPI;
            try
            {
                luisResponseData = (await this.GetFromLuisAsync(userRequestMessage));
                queryForAPI = ConstructQueryHelper(luisResponseData);
                processedIntent = luisResponseData.TopScoringIntent;
                responseFromAPI = await this.GetFromApiAsync(queryForAPI, processedIntent.Intent);
            }
            catch (Exception ex)
            {
                if (ex is NullReferenceException)
                {
                    return "Can't proccess the query, ICB API can't find data for the query!";
                }
                return ex.Message;
            }

            string entity;
            string entityID;
            string partNumber = string.Empty;
            string responseValue = responseFromAPI.Value;

            if (processedIntent.Intent == "MachineRequestData")
            {
                MachineRequestData machineRequestObj = JsonConvert.DeserializeObject<MachineRequestData>(queryForAPI);
                entity = machineRequestObj.Type;
                entityID = machineRequestObj.SensorID;
            }
            else
            {
                KPIRequestDataWithPart kpiRequestObj = JsonConvert.DeserializeObject<KPIRequestDataWithPart>(queryForAPI);
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
