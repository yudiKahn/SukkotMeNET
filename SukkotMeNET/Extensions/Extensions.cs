using SukkotMeNET.Models;

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
            var res = (T)Activator.CreateInstance(obj.GetType());

            foreach (var prop in obj.GetType().GetProperties())
            {
                prop.SetValue(res, prop.GetValue(obj));
            }

            return res;
        }
    }
}
