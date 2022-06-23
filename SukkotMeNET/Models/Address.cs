using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace SukkotMeNET.Models
{
    public class Address
    {
        [Required]
        [BsonElement("street")]
        public string Street { get; set; }

        [Required]
        [BsonElement("city")]
        public string City { get; set; }

        [Required]
        [BsonElement("state")]
        public string State { get; set; }

        [Required]
        [BsonElement("zip")]
        public int Zip { get; set; }

        public override string ToString() => $"{Street} St {City} {State} {Zip}";
    }
}
