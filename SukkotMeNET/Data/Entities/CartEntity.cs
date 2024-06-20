using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using IEsrog.Models;
using IEsrog.Data.Interfaces;

namespace IEsrog.Data.Entities;

[BsonIgnoreExtraElements]
public class CartEntity 
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonElement("id")]
    public string Id { get; set; } = string.Empty;

    [BsonRepresentation(BsonType.ObjectId)]
    [BsonElement("userId")]
    public string UserId { get; set; } = string.Empty;

    [BsonElement("items")]
    public List<OrderItemEntity> Items { get; set; } = new();

    public string Comment { get; set; } = string.Empty;
}