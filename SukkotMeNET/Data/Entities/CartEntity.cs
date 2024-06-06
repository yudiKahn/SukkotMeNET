using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using SukkotMeNET.Models;
using SukkotMeNET.Data.Interfaces;

namespace SukkotMeNET.Data.Entities;

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