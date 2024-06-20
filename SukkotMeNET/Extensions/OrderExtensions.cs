using IEsrog.Models;

namespace IEsrog.Extensions
{
    public static class OrderExtensions
    {
        public static double GetTotal(this Order order)
        {
            double total = 0;
            foreach (var item  in order.Items)
            {
                total += item.Price * item.Qty;
            }
            return total;
        }
    }
}
