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
        public string Id { get; set; } = string.Empty;

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        /*[BsonElement("category")]
        public string Category { get; set; }*/

        [BsonElement("prices")] 
        public double[] Prices { get; set; } = Array.Empty<double>();

        [BsonElement("pricesTypes")]
        public string[] PricesTypes { get; set; } = Array.Empty<string>();

        [BsonElement("options")]
        public string[] Options { get; set; } = Array.Empty<string>();

        public override string ToString() => Name;
    }
}
