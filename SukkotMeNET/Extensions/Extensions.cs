using SukkotMeNET.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SukkotMeNET.Extensions
{
    public static class Extensions
    {
        #region Item Extensions

        public static string GetItemIcon(this Item item)
        {

            var name = $"{item.Name.ToLower()}";
            if (name.Contains("lulav"))
                return "/images/lulav.png";
            else if (name.Contains("hadas"))
                return "/images/hadas.png";
            else if (name.Contains("aruvos") || name.Contains("hoshnos"))
                return "/images/harava.png";
            else if (name.Contains("set"))
                return "/images/set.png";
            else if (name.Contains("schach"))
                return "/images/schach.png";
            else
                return "/images/esrog.png";
        }

        #endregion

        public static OrderItem ToOrderItem(this Item item, int optionIndex, int priceIndex, int qty = 0)
        {
            return new OrderItem()
            {
                Id = item.Id,
                Name = item.Name,
                Option = item.Options.ElementAtOrDefault(optionIndex),
                Price = item.Prices.ElementAtOrDefault(priceIndex),
                PriceType = item.PricesTypes.ElementAtOrDefault(priceIndex),
                Qty = qty,
            };
        }

        public static T Clone<T>(this T obj)
        {
            T res = (T)Activator.CreateInstance(obj.GetType());

            foreach (var prop in obj.GetType().GetProperties())
            {
                prop.SetValue(res, prop.GetValue(obj));
            }

            return res;
        }
    }
}
