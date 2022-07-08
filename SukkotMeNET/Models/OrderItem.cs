using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SukkotMeNET.Models
{
    [BsonIgnoreExtraElements]
    public class OrderItem
    {

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("id")]
        public string Id { get; set; } = string.Empty;

        [BsonElement("category")]
        public string Category { get; set; } = string.Empty;

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("price")]
        public double Price { get; set; }

        [BsonElement("priceType")]
        public string? PriceType { get; set; }

        [BsonElement("option")]
        public string? Option { get; set; }
        
        [BsonElement("q")]
        public int Qty { get; set; }

        [BsonElement("byAdmin")]
        public bool ByAdmin { get; set; }

        public override string ToString() => $"{Name} {PriceType} {Option}";
    }
}
