using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static WebAPITest.Models.Query;

namespace WebAPITest.Models
{
    public class LuisResponse
    {
        public List<Entity> Entities { get; set; }
        public List<Intent> Intents { get; set; }
        public TopScoringIntent TopScoringIntent { get; set; }
        public string Query { get; set; }
        public List<CompositeEntity> CompositeEntities { get; set; }
    }
}
