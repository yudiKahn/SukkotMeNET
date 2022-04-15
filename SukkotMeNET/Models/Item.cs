using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SukkotMeNET.Models
{
    [BsonIgnoreExtraElements]
    public class Item
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("id")]
        public string Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("category")]
        public string Category { get; set; }

        [BsonElement("prices")]
        public double[] Prices { get; set; }

        [BsonElement("pricesTypes")]
        public string[] PricesTypes { get; set; }

        [BsonElement("options")]
        public string[] Options { get; set; }
    }
}
