using IEsrog.Data.Entities;
using IEsrog.Models;

namespace IEsrog.Extensions;

public static class ToModelExtensions
{
    public static Product ToModel(this ProductEntity entity)
    {
        return new Product
        {
            Id = entity.Id,
            Name = entity.Name,
            Category = entity.Category.ToString(),
            Price = entity.Price,
            PricesType = entity.PriceType,
            Options = entity.Options,
            Group = entity.Group,
            ExtraOptions = entity.ExtraOptions?.Select(x => x.ToModel()).ToArray(),
            Includes = entity.Includes?
                .Select(i => new ProductInclude()
                {
                    ProductId = i.ProductId,
                    Qty = i.Qty
                })
                .ToArray()
        };
    }

    public static ExtraOptions ToModel(this ExtraOptionsEntity e)
    {
        return new ExtraOptions()
        {
            Option = e.Option,
            Price = e.Price
        };
    }

    public static OrderItem ToModel(this Product prod, string? opt, string? priceType, int qty = 1)
    {
        var okOpt = prod.Options?.Contains(opt) == true;
        return new OrderItem
        {
            ProductId = prod.Id,
            Category = prod.Category,
            Name = prod.Name,
            Price = prod.Price,
            PriceType = prod.Group == 14 ? priceType : prod.PricesType, //todo
            Option = okOpt ? opt : string.Empty,
            Qty = qty, //todo 
            ByAdmin = false,
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
            ByAdmin = entity.ByAdmin,
            ProductId = entity.ProductId,
            ExtraOption = entity.ExtraOption?.ToModel()
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
            ShippingCost = entity.ShippingCost,
            Recipient = entity.Recipient
        };
    }
}