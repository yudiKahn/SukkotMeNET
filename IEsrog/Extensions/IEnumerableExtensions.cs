using IEsrog.Models;

namespace IEsrog.Extensions
{
    public static class IEnumerableExtensions
    {
        public static string GetFriendlyRange(this double[] values,char currency = '$')
        {
            if (!values.Any())
                return string.Empty;
            return values.Length == 1 ? $"{currency}{values[0]}" : $"{currency}{values[0]} - {currency}{values[^1]}";
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

       

        public static void AddOrMerge(this List<OrderItem> items, OrderItem newItem, bool toOverride = false)
        {
            //var existItem = items.FirstOrDefault(i => 
            //    i.Name == newItem.Name && i.Option == newItem.Option && Math.Abs(i.Price - newItem.Price) == 0.0 && i.PriceType == newItem.PriceType && i.ByAdmin == newItem.ByAdmin);


            var existItem = items.FirstOrDefault(i =>
                i.Option == newItem.Option && i.PriceType == newItem.PriceType && 
                i.ByAdmin == newItem.ByAdmin && i.ProductId == newItem.ProductId);

            if (existItem == null && newItem.Qty > 0)
            {
                items.Add(newItem);
            }
            else if(existItem != null)
            {
                existItem.Qty += newItem.Qty;
                if (toOverride)
                {
                    existItem.Qty = newItem.Qty;
                }
                if (existItem.Qty < 1)
                {
                    items.Remove(existItem);
                }
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
