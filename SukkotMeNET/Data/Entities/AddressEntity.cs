using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace SukkotMeNET.Data.Entities;

public class AddressEntity
{
    [Required]
    [BsonElement("street")]
    public string Street { get; set; } = string.Empty;

    [Required]
    [BsonElement("city")]
    public string City { get; set; } = string.Empty;

    [Required]
    [BsonElement("state")]
    public string State { get; set; } = string.Empty;

    [Required]
    [BsonElement("zip")]
    public int Zip { get; set; }

    public override string ToString() => $"{Street} St {City} {State} {Zip}";
}