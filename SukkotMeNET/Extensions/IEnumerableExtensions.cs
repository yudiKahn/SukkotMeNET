using SukkotMeNET.Models;

namespace SukkotMeNET.Extensions
{
    public static class IEnumerableExtensions
    {
        public static string GetFriednlyRange<T>(this IEnumerable<T> numbers,char currency = '$')
        {
            if (!numbers.Any())
                return string.Empty;
            else if (numbers.Count() == 1)
                return $"{currency}{numbers.ElementAt(0)}";
            return $"{currency}{numbers.ElementAt(0)} - {currency}{numbers.ElementAt(numbers.Count() - 1)}";
        }

        public static double GetTotal(this IEnumerable<OrderItem> items)
        {
            var res = 0.0;

            foreach(var item in items)
            {
                res += (item.Qty * item.Price);
            }

            return res;
        }

        public static void AddOrMerge(this List<OrderItem> items, OrderItem newItem)
        {
            var existItem = items.FirstOrDefault(i => i.Name == newItem.Name && i.Option == newItem.Option && i.Price == newItem.Price && i.PriceType == newItem.PriceType);
            
            if(existItem != null)
            {
                existItem.Qty += newItem.Qty;
            }
            else
            {
                items.Add(newItem);
            }
        }
    }
}
