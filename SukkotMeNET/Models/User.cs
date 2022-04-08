using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace SukkotMeNET.Models
{
    [BsonIgnoreExtraElements]
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("id")]
        public string Id { get; set; }

        [Required]
        [BsonElement("firstName")]
        public string FirstName { get; set; }

        [Required]
        [BsonElement("lastName")]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        [BsonElement("email")]
        public string Email { get; set; }

        [Required]
        [BsonElement("phoneNumber")]
        public string PhoneNumber { get; set; }

        [Required]
        [StringLength(30, ErrorMessage = "Password must be between 8 and 30 characters.", MinimumLength = 8)]
        [BsonElement("password")]
        public string Password { get; set; }

        [BsonElement("isAdmin")]
        public bool IsAdmin { get; }

        [Required]
        [BsonElement("address")]
        public Address Address { get; set; }
    }
}
