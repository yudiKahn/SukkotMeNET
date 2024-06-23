using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace IEsrog.Models
{
    [BsonIgnoreExtraElements]
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("id")]
        public string Id { get; set; } = string.Empty;

        [Required]
        [BsonElement("firstName")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [BsonElement("lastName")]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [BsonElement("email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [BsonElement("phoneNumber")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(30, ErrorMessage = "Password must be between 8 and 30 characters.", MinimumLength = 8)]
        [BsonElement("password")]
        public string Password { get; set; } = string.Empty;

        [BsonElement("isAdmin")]
        public bool IsAdmin { get; set; }

        [Required] 
        [BsonElement("address")] 
        public Address Address { get; set; } = new();

        public override string ToString() => $"{FirstName} {LastName}";
    }
}
