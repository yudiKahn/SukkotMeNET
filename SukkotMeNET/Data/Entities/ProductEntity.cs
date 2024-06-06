using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using SukkotMeNET.Data.Interfaces;

namespace SukkotMeNET.Data.Entities;

[BsonIgnoreExtraElements]
public class ProductEntity 
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonElement("id")]
    public string Id { get; set; } = string.Empty;

    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("category")]
    public required string Category { get; set; }

    [BsonElement("prices")]
    public double[] Prices { get; set; } = [];

    [BsonElement("pricesTypes")]
    public string[] PricesTypes { get; set; } = [];

    [BsonElement("options")]
    public string[] Options { get; set; } = [];

    public override string ToString() => Name;
}