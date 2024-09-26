using IEsrog.Models;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace IEsrog.Data.Entities;

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
    
    [BsonElement("extraOptions")]
    public ExtraOptionsEntity[]? ExtraOptions { get; set; }

    [BsonElement("group")]
    public byte Group { get; set; }

    [BsonElement("includes")]
    public ProductIncludeEntity[]? Includes { get; set; }

    public override string ToString() => Name;
}

public class ExtraOptionsEntity
{
    [BsonElement("option")]
    public required string Option { get; set; }
    
    [BsonElement("price")]
    public double Price { get; set; }
}

public class ProductIncludeEntity
{
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonElement("productId")]
    public string ProductId { get; set; } = string.Empty;
    
    [BsonElement("qty")]
    public int Qty { get; set; }
}