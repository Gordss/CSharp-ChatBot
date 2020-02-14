using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPITest.Models
{
    public class Query
    {
        public class TopScoringIntent
        {
            public string Intent { get; set; }
            public double Score { get; set; }

            public bool nonOrderQuery()
            {
                return this.Intent != "OrderRemainingWorkDeviation" &&
                       this.Intent != "OrderEstimatedRemainigWork" &&
                       this.Intent != "OrderActualRemainigWork";
            }
        }

        public class Intent
        {
            public string IntentValue { get; set; }
            public double Score { get; set; }
        }

        public class Resolution
        {
            public List<string> values { get; set; }
        }

        public class Entity
        {
            public string entity { get; set; }
            public string Type { get; set; }
            public int StartIndex { get; set; }
            public int EndIndex { get; set; }
            public Resolution resolution { get; set; }

        }

        public class LuisResponse
        {
            public string MessageQuery { get; set; }
            public TopScoringIntent TopScoringIntent { get; set; }
            public List<Intent> Intents { get; set; }
            public List<Entity> Entities { get; set; }
            public List<CompositeEntity> CompositeEntities { get; set; }

            public LuisResponse(List<Entity> entities, List<Intent> intents, TopScoringIntent topScoringIntent, string query, List<CompositeEntity> compositeEntities)
            {
                Entities = entities;
                Intents = intents;
                TopScoringIntent = topScoringIntent;
                MessageQuery = query;
                CompositeEntities = compositeEntities;
            }
            public LuisResponse(LuisResponse data) : this(data.Entities, data.Intents, data.TopScoringIntent, data.MessageQuery, data.CompositeEntities) { }
            public LuisResponse() : this(new List<Entity>(), new List<Intent>(), new TopScoringIntent(), "", new List<CompositeEntity>()) { }
        }

        public class CompositeEntitiesChildren
        {
            public string Type { get; set; }

            public string Value { get; set; }
        }

        public class CompositeEntity
        {
            public string ParentType { get; set; }

            public string Value { get; set; }
            public List<CompositeEntitiesChildren> Children { get; set; }
        }
    }
}
