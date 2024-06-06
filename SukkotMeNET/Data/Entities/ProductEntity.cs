using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using SukkotMeNET.Data.Interfaces;
using SukkotMeNET.Models;

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
    public ProductCategory Category { get; set; }

    [BsonElement("prices")]
    public double[] Prices { get; set; } = [];

    [BsonElement("pricesTypes")]
    public string[] PricesTypes { get; set; } = [];

    [BsonElement("options")]
    public string[] Options { get; set; } = [];
    
    public ProductIncludeEntity[]? Includes { get; set; }

    public override string ToString() => Name;
}

public class ProductIncludeEntity
{
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonElement("productId")]
    public string ProductId { get; set; } = string.Empty;
    
    [BsonElement("priceInx")]
    public int PriceIndex { get; set; }
    
    [BsonElement("qty")]
    public float Qty { get; set; }
}