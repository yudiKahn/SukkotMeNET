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

    [BsonElement("price")]
    public double Price { get; set; }

    [BsonElement("priceType")]
    public string PriceType { get; set; } = string.Empty;

    [BsonElement("options")]
    public string[]? Options { get; set; }

    [BsonElement("group")]
    public byte Group { get; set; }

    [BsonElement("includes")]
    public ProductIncludeEntity[]? Includes { get; set; }

    public override string ToString() => Name;
}

public class ProductIncludeEntity
{
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonElement("productId")]
    public string ProductId { get; set; } = string.Empty;
    
    [BsonElement("qty")]
    public int Qty { get; set; }
}