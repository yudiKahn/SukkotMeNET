using MongoDB.Bson.Serialization.Attributes;

namespace SukkotMeNET.Models
{
    public class Address
    {
        [BsonElement("street")]
        public string Street { get; set; }

        [BsonElement("city")]
        public string City { get; set; }

        [BsonElement("state")]
        public string State { get; set; }

        [BsonElement("zip")]
        public int Zip { get; set; }
    }
}
