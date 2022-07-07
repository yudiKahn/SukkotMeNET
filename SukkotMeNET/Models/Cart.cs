using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SukkotMeNET.Models
{
    [BsonIgnoreExtraElements]
    public class Cart
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("id")]
        public string Id { get; set; } = string.Empty;

        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("userId")]
        public string UserId { get; set; } = string.Empty;

        [BsonElement("items")]
        public List<OrderItem> Items { get; set; }

        public string Comment { get; set; } = string.Empty;

        public Cart()
        {
            Items = new();
        }
    }
}
