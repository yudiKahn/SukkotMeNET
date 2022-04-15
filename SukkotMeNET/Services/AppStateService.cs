using MongoDB.Bson;
using SukkotMeNET.Models;

namespace SukkotMeNET.Services
{
    public class AppStateService
    {
        public User User { get; set; }

        public Cart Cart { get; set; }

        public List<Order> Orders { get; set; }

        public AppStateService()
        {
            Orders = new List<Order>();
            //TODO:remove
            User = new User() //TODO remove
            {
                Email = "yudik@gmail.com",
                Address = new Address()
                {

                },
                FirstName = "Yudi",
                LastName = "Kahn",
                Id = ObjectId.GenerateNewId().ToString(),
                Password = "123",
                PhoneNumber = "123",
                IsAdmin = true
            };
        }
    }
}
