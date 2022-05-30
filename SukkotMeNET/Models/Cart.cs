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
        public string Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("userId")]
        public string UserId { get; set; }

        [BsonElement("items")]
        public List<OrderItem> Items { get; set; }

        public List<OrderItem> SaleItems { get; set; }

        public Cart()
        {
            Items = new();
            SaleItems = new();
        }
    }
}
