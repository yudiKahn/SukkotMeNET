﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SukkotMeNET.Models
{
    [BsonIgnoreExtraElements]
    public class Order
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("id")]
        public string Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("userId")]
        public string UserId { get; set; }

        [BsonElement("items")]
        public List<OrderItem> Items { get; set; }

        [BsonElement("comment")]
        public string Comment { get; set; }

        [BsonElement("isPaid")]
        public bool IsPaid { get; set; }

        [BsonElement("isDone")]
        public bool IsPacked { get; set; }

        [BsonElement("isShipped")]
        public bool IsShipped { get; set; }

        [BsonElement("CreatedAt")]
        public DateTime CreatedAt { get; set; }

        [BsonElement("createdAt")]
        public DateTime createdAt { get; set; }

    }
}
