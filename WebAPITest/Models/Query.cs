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
            public string Intent { get; set; } //Rename it to IntentValue
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
            public List<string> Value { get; set; }
        }

        public class Entity
        {
            public string entity { get; set; } //Rename to EntityValue
            public string Type { get; set; }
            public int StartIndex { get; set; }
            public int EndIndex { get; set; }
            public Resolution resolution { get; set; }

            //public Entity(string _entity, string type, int startIndex, int endIndex, Resolution _resolution)
            //{
            //    entity = _entity;
            //    Type = type;
            //    StartIndex = startIndex;
            //    EndIndex = endIndex;
            //    resolution = _resolution;
            //}

        }

        public class LuisResponse
        {
            public string Query { get; set; }
            public TopScoringIntent TopScoringIntent { get; set; }
            public List<Intent> Intents { get; set; }
            public List<Entity> Entities { get; set; }
            public List<CompositeEntity> CompositeEntities { get; set; }

            public LuisResponse(List<Entity> entities, List<Intent> intents, TopScoringIntent topScoringIntent, string query, List<CompositeEntity> compositeEntities)
            {
                Entities = entities;
                Intents = intents;
                TopScoringIntent = topScoringIntent;
                Query = query;
                CompositeEntities = compositeEntities;
            }
            public LuisResponse(LuisResponse data) : this(data.Entities, data.Intents, data.TopScoringIntent, data.Query, data.CompositeEntities) { }
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
