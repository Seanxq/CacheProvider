using System;
using CacheProvider.Model;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CacheProvider.Mongo.Model
{
    [Serializable]
    public class CacheItem: BaseModel
    {
        public CacheItem()
        {
            Id = ObjectId.GenerateNewId().ToString();
        }

        [BsonId]
        public string Id { get; set; }
    }
}