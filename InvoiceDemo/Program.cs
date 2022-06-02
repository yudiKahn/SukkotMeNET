using SukkotMeNET.Models;
using SukkotMeNET.Services;

User u = new User()
{
    Email = "dani@gmail.com",
    FirstName = "Dani",
    LastName = "Kahn",
    PhoneNumber = "0559991015",
    Address = new Address()
    {
        City = "City",
        State = "State",
        Street = "Street",
        Zip = 123
    }
};

Order o = new Order()
{
    Comment = "Comment..",
    CreatedAt = DateTime.Now,
    Items = new()
    {
        new OrderItem()
        {
            Name = "Esrog",
            Qty = 10,
            Price = 10
        },
        new OrderItem()
        {
            Name = "Lulav",
            Qty = 20,
            Price = 12
        },
    }
};

InvoiceHelper.GetInvoiceHTML(o, u);