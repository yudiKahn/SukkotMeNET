using MongoDB.Bson;
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
        public IEnumerable<OrderItem> Items { get; set; }

        [BsonElement("comment")]
        public string Comment { get; set; }

        [BsonElement("isPaid")]
        public bool IsPaid { get; set; }

        [BsonElement("isDone")]
        public bool IsDone { get; set; }

        [BsonDateTimeOptions]
        public DateTime CreatedAt { get; set; }
    }
}
