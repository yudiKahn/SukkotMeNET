﻿using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using IEsrog.Models;

namespace IEsrog.Data.Entities;

[BsonIgnoreExtraElements]
public class OrderItemEntity
{

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonElement("id")]
    public string Id { get; set; } = string.Empty;
    
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonElement("productId")]
    public string? ProductId { get; set; }//todo: impl

    [BsonElement("category")]
    public string Category { get; set; } = string.Empty; //todo: can be removed

    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;

    [BsonElement("price")]
    public double Price { get; set; }

    [BsonElement("priceType")]
    public string? PriceType { get; set; }

    [BsonElement("option")]
    public string? Option { get; set; }

    [BsonElement("extraOption")]
    public ExtraOptionsEntity? ExtraOption { get; set; }

    [BsonElement("q")]
    public int Qty { get; set; }

    [BsonElement("byAdmin")]
    public bool ByAdmin { get; set; }

    public override string ToString() => $"{Name} {PriceType} {Option}";
}