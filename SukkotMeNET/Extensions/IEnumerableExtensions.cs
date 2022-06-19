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

        public static List<OrderItem> GetWithSaleItems(this List<OrderItem> items)
        {
            var itemsToAdd = new List<OrderItem>(items);

            //add 20% extra for israeli sets
            var israeliSet = itemsToAdd.FirstOrDefault(i => i.Id == Constants.General.IsraeliSetItemId);
            if (israeliSet is not null)
            {
                var qtyToAdd = (int)(0.2 * israeliSet.Qty);
                itemsToAdd.AddOrMerge(new OrderItem()
                {
                    Id = israeliSet.Id,
                    ByAdmin = true,
                    Category = israeliSet.Category,
                    Name = israeliSet.Name,
                    Option = israeliSet.Option,
                    Price = 0,
                    PriceType = israeliSet.PriceType,
                    Qty = qtyToAdd
                });
            }

            return itemsToAdd;
        }

        public static void AddOrMerge(this List<OrderItem> items, OrderItem newItem)
        {
            var existItem = items.FirstOrDefault(i => i.Name == newItem.Name && i.Option == newItem.Option && i.Price == newItem.Price && i.PriceType == newItem.PriceType && i.ByAdmin == newItem.ByAdmin);
            
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
