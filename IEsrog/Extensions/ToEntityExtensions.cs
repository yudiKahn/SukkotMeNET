using IEsrog.Data.Entities;
using IEsrog.Models;
using MongoDB.Bson;
using SharpCompress.Common;

namespace IEsrog.Extensions;

public static class ToEntityExtensions
{
    public static ProductEntity ToEntity(this Product model)
    {
        return new ProductEntity
        {
            Id = model.Id,
            Name = model.Name,
            Category = Enum.Parse<ProductCategory>(model.Category),
            Price = model.Price,
            PriceType = model.PricesType,
            Options = model.Options,
            ExtraOptions = model.ExtraOptions?.Select(x => x.ToEntity()).ToArray(),
            Includes = model.Includes?
                .Select(i => new ProductIncludeEntity()
                {
                    ProductId = i.ProductId,
                    Qty = i.Qty
                })
                .ToArray()
        };
    }

    public static ExtraOptionsEntity ToEntity(this ExtraOptions e)
    {
        return new ExtraOptionsEntity()
        {
            Option = e.Option,
            Price = e.Price
        };
    }

    public static UserEntity ToEntity(this User model)
    {
        return new UserEntity
        {
            Id = model.Id,
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email,
            PhoneNumber = model.PhoneNumber,
            Password = model.Password,
            IsAdmin = model.IsAdmin,
            Address = model.Address.ToEntity()
        };
    }

    public static AddressEntity ToEntity(this Address model)
    {
        return new AddressEntity
        {
            Street = model.Street,
            City = model.City,
            State = model.State,
            Zip = model.Zip
        };
    }

    public static CartEntity ToEntity(this Cart model)
    {
        return new CartEntity
        {
            Id = model.Id,
            UserId = model.UserId,
            Items = model.Items.Select(i => i.ToEntity()).ToList(),
            Comment = model.Comment
        };
    }

    public static OrderItemEntity ToEntity(this OrderItem model)
    {
        if (string.IsNullOrWhiteSpace(model.Id))
        {
            model.Id = ObjectId.GenerateNewId().ToString();
        }
        return new OrderItemEntity
        {
            Id = model.Id,
            Category = model.Category,
            Name = model.Name,
            Price = model.Price,
            PriceType = model.PriceType,
            Option = model.Option,
            Qty = model.Qty,
            ByAdmin = model.ByAdmin,
            ProductId = model.ProductId.StrOrNull(),
            ExtraOption = model.ExtraOption?.ToEntity()
        };
    }

    public static OrderEntity ToEntity(this Order model)
    {
        return new OrderEntity
        {
            Id = model.Id,
            UserId = model.UserId,
            Items = model.Items.Select(i => i.ToEntity()).ToList(),
            Comment = model.Comment,
            IsPaid = model.IsPaid,
            IsPacked = model.IsPacked,
            IsShipped = model.IsShipped,
            CreatedAt = model.CreatedAt,
            ShippingCost = model.ShippingCost,
            Recipient = model.Recipient
        };
    }
}