using IEsrog.Models;

namespace IEsrog.Extensions
{
    public static class Extensions
    {
        #region Product Extensions

        public static string GetItemIcon(this Product product)
        {

            var name = $"{product.Name.ToLower()}";
            if (name.Contains("lulav"))
                return "/images/lulav.png";
            if (name.Contains("hadas"))
                return "/images/hadas.png";
            if (name.Contains("aruvos") || name.Contains("hoshnos"))
                return "/images/harava.png";
            if (name.Contains("set"))
                return "/images/set.png";
            if (name.Contains("schach"))
                return "/images/schach.png";
            if (name.Contains("koisaklach"))
                return "/images/koishklach.png";
            if (name.Contains("sukkah"))
                return "/images/sukkah.png";

            return "/images/esrog.png";
        }

        #endregion

        public static OrderItem ToOrderItem(this Product product, int optionIndex, int priceIndex, int qty = 0)
        {
            return new OrderItem()
            {
                Id = product.Id,
                Name = product.Name,
                Option = product.Options.ElementAtOrDefault(optionIndex),
                Price = product.Price,//s.ElementAtOrDefault(priceIndex),
                PriceType = product.PricesType,//s.ElementAtOrDefault(priceIndex),
                Qty = qty,
            };
        }

        public static T Clone<T>(this T obj) where T : class, new()
        {
            var res = new T();

            foreach (var prop in obj.GetType().GetProperties())
            {
                prop.SetValue(res, prop.GetValue(obj));
            }

            return res;
        }

        public static Order Clone(this Order o)
        {
            return new Order()
            {
                Comment = o.Comment,
                CreatedAt = o.CreatedAt,
                Id = o.Id,
                IsPacked = o.IsPacked,
                IsPaid = o.IsPaid,
                IsShipped = o.IsShipped,
                ShippingCost = o.ShippingCost,
                UserId = o.UserId,
                Items = o.Items.Select(i => i.Clone()).ToList()
            };
        }
    }
}
