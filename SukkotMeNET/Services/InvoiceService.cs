using SukkotMeNET.Extensions;
using SukkotMeNET.Models;
using System.Text.RegularExpressions;
using SukkotMeNET.Pages;

namespace SukkotMeNET.Services
{
    public class InvoiceService
    {
        readonly AppStateService _AppState;

        public InvoiceService(AppStateService appState)
        {
            _AppState = appState;
        }

        public string GetInvoiceHtml(Order order, User user)
        {
            var invoicePath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "invoice.html");
            if (!File.Exists(invoicePath)) return string.Empty;

            var html = File.ReadAllText(invoicePath);

            Dictionary<string, string> htmlValues = new()
            {
                {"OrderId", order.Id },
                {"Created", order.CreatedAt.ToString("MMMM dd yyyy hh:mm") },
                {"UserName", $"{user.FirstName} {user.LastName}"  },
                {"UserEmail", user.Email },
                {"UserPhone", user.PhoneNumber },
                {"UserAddress", user.Address?.ToString() ?? "" },
                {"Total", order.Items.GetTotal().ToString("N2") },
                {"Items", string.Join(' ',order.Items.OrderBy(i => i.Name).Select(item =>
                $@"<tr>
                        <td><input type={'"'}checkbox{'"'}/> {item.Name} <small>{item.PriceType} {item.Option}</small></td>
                        <td style={'"'}text-align:right;{'"'}>{item.Qty}</td>
                        <td style={'"'}text-align:right;{'"'}>${item.Price}</td>
                    </tr>"

                )) },
                {"SetsItems", string.Join(' ', GetItemsIncludedInSets(order.Items).OrderBy(i => i.Name).Select(item => 
                    $@"<tr>
                        <td colspan={'"'}3{'"'}>
                            <input type={'"'}checkbox{'"'}/> {item.Name} <small>{item.PriceType} {item.Option}</small>
                        </td>
                        <td>{item.Qty}</td>
                    </tr>")) },
                {"PaymentMethod", "Check" }
            };

            var newHtml = Regex.Replace(html, @"\[(.+?)\]", m => htmlValues[m.Groups[1].Value.Trim()]);

            return newHtml;

        }


        public List<OrderItem> GetItemsIncludedInSets(List<OrderItem> items)
        {
            var res = new List<OrderItem>();
            var sets = items.Where(i => i.Id is Constants.General.IsraeliSetItemId or Constants.General.YaneverSetItemId);

            //When ordering a set the customer giraeliSetQtyets:
            //esrog, Egyptien lulav, hadas רובו חיים נאה,  aruvos, koishaklach & plastic bag
            //for yanever set over 50$ he gets also hadas כולו חיים נאה
            var lulav = _AppState.ShopItems.FirstOrDefault(i => i.Id == "6176e562654d28089f656e1d");
            var hadas = _AppState.ShopItems.FirstOrDefault(i => i.Id == "6176e5d3654d28089f656e25");
            var arhava = _AppState.ShopItems.FirstOrDefault(i => i.Id == "6176e589654d28089f656e21");
            var koishaklach = _AppState.ShopItems.FirstOrDefault(i => i.Id == "6176e645514ed3ae2d40d911");
            var plasticBag = _AppState.ShopItems.FirstOrDefault(i => i.Id == "6176e5f0654d28089f656e29");

            foreach (var set in sets)
            {
                res.AddOrMergeRange(new OrderItem[]
                {
                    lulav.ToOrderItem(0, lulav.Prices.Length-1, set.Qty),
                    hadas.ToOrderItem(0, hadas.Prices.Length-1, set.Qty),
                    arhava.ToOrderItem(0, 0, set.Qty),
                    koishaklach.ToOrderItem(0, 0, set.Qty),
                    plasticBag.ToOrderItem(0, 0, set.Qty),
                    new OrderItem()
                    {
                        Id = set.Id,
                        Name = set.Name.Replace("set", "Esrog", StringComparison.OrdinalIgnoreCase),
                        Option = set.Option,
                        PriceType = set.PriceType,
                        Qty = set.Qty
                    }
                });
            }


            foreach (var set in sets.Where(s => s.Id == Constants.General.YaneverSetItemId && s.Price >= 50))
            {
                res.AddOrMerge(hadas.ToOrderItem(0, hadas.Prices.Length - 2, set.Qty));
            }

            return res;
        }
    }
}
