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

        public static double GetTotal(this IEnumerable<OrderItem> items, double shipment = 0D)
        {
            var res = shipment;

            foreach(var item in items)
            {
                res += (item.Qty * item.Price);
            }

            return res;
        }

       

        public static void AddOrMerge(this List<OrderItem> items, OrderItem newItem)
        {
            var existItem = items.FirstOrDefault(i => 
                i.Name == newItem.Name && i.Option == newItem.Option && Math.Abs(i.Price - newItem.Price) == 0.0 && i.PriceType == newItem.PriceType && i.ByAdmin == newItem.ByAdmin);
            
            if(existItem == null)
            {
                items.Add(newItem);
            }
            else
            {
                existItem.Qty += newItem.Qty;
            }
        }

        public static void AddOrMergeRange(this List<OrderItem> items, params OrderItem[] newItems)
        {
            foreach(var item in newItems)
            {
                items.AddOrMerge(item);
            }
        }
    }
}
