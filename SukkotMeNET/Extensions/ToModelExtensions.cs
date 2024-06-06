using SukkotMeNET.Data.Entities;
using SukkotMeNET.Models;

namespace SukkotMeNET.Extensions;

public static class ToModelExtensions
{
    public static Item ToModel(this ProductEntity entity)
    {
        return new Item
        {
            Id = entity.Id,
            Name = entity.Name,
            Category = entity.Category,
            Prices = entity.Prices,
            PricesTypes = entity.PricesTypes,
            Options = entity.Options
        };
    }

    public static User ToModel(this UserEntity entity)
    {
        return new User
        {
            Id = entity.Id,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            Email = entity.Email,
            PhoneNumber = entity.PhoneNumber,
            Password = entity.Password,
            IsAdmin = entity.IsAdmin,
            Address = entity.Address.ToModel()
        };
    }

    public static Address ToModel(this AddressEntity entity)
    {
        return new Address
        {
            Street = entity.Street,
            City = entity.City,
            State = entity.State,
            Zip = entity.Zip
        };
    }

    public static Cart ToModel(this CartEntity entity)
    {
        return new Cart
        {
            Id = entity.Id,
            UserId = entity.UserId,
            Items = entity.Items.Select(i => i.ToModel()).ToList(),
            Comment = entity.Comment
        };
    }

    public static OrderItem ToModel(this OrderItemEntity entity)
    {
        return new OrderItem
        {
            Id = entity.Id,
            Category = entity.Category,
            Name = entity.Name,
            Price = entity.Price,
            PriceType = entity.PriceType,
            Option = entity.Option,
            Qty = entity.Qty,
            ByAdmin = entity.ByAdmin
        };
    }

    public static Order ToModel(this OrderEntity entity)
    {
        return new Order
        {
            Id = entity.Id,
            UserId = entity.UserId,
            Items = entity.Items.Select(i => i.ToModel()).ToList(),
            Comment = entity.Comment,
            IsPaid = entity.IsPaid,
            IsPacked = entity.IsPacked,
            IsShipped = entity.IsShipped,
            CreatedAt = entity.CreatedAt,
            ShippingCost = entity.ShippingCost
        };
    }
}