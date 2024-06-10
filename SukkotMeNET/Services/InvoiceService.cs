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
                {"OrderId", order.Id[..6] },
                {"OrderIdFull", order.Id },
                {"Created", order.CreatedAt.ToString("MMMM dd yyyy hh:mm") },
                {"UserName", $"{user.FirstName} {user.LastName}"  },
                {"UserEmail", user.Email },
                {"UserPhone", user.PhoneNumber },
                {"UserAddress", user.Address?.ToString() ?? "" },
                {"Total", order.Items.GetTotal(order.ShippingCost).ToString("N2") },
                {"Items", string.Join(' ',order.Items.OrderBy(i => i.Name).Select(item =>
                $@"<tr>
                        <td><input type={'"'}checkbox{'"'}/> {item.Name} <small>{item.PriceType} {item.Option}</small></td>
                        <td>{item.Qty}</td>
                        <td>${item.Price}</td>
                        <td style={'"'}text-align:right;{'"'}>${item.Qty * item.Price:N2}</td>
                    </tr>"

                )) },
                {"ItemsInTheBox", string.Join(' ', GetItemsInTheBox(order.Items).OrderBy(i => i.Name).Select(item => 
                    $@"<tr>
                        <td colspan={'"'}3{'"'}>
                            <input type={'"'}checkbox{'"'}/> {item.Name} <small>{item.PriceType} {item.Option}</small>
                        </td>
                        <td style={'"'}text-align:right;{'"'}>{item.Qty}</td>
                    </tr>")) },
                { "ShippingCost", $"${order.ShippingCost:N2}" },
            };

            var newHtml = Regex.Replace(html, @"\[(.+?)\]", m => 
                htmlValues.TryGetValue(m.Groups[1].Value.Trim(), out var v) ? v : "");

            return newHtml;

        }

        public List<OrderItem> GetItemsInTheBox(List<OrderItem> items)
        {
            //todo: match by price in doc. Not current price
            //todo: mthod invokes few times when requested from UI
            var map = _AppState.Products
                .ToDictionary(k => k.Id);

            var res = new List<OrderItem>(items.Count);
            foreach (var item in items)
            {
                if (map.TryGetValue(item.ProductId, out var p) &&
                    p.Includes?.Any() == true)
                {
                    var opt = item.Option;
                    var inc = p.Includes
                        .Select(i => map[i.ProductId].ToModel(opt, i.Qty))
                        .ToArray();
                    
                    res.AddOrMergeRange(inc);
                }
                else
                {
                    res.AddOrMerge(item);
                }
            }

            return res;
        }
        
        
        //todo: make obsolete
        public List<OrderItem> _GetItemsInTheBox(List<OrderItem> items)
        {
            //When ordering a set the customer gets:
            //esrog, Egyptien lulav, hadas רובו חיים נאה,  aruvos, koishaklach & plastic bag
            //for yanever set over 50$ he gets also hadas כולו חיים נאה
            //
            //for each israeli set 20% lulavim extra
            var lulav = _AppState.Products.FirstOrDefault(i => i.Id == "6176e562654d28089f656e1d");
            var hadas = _AppState.Products.FirstOrDefault(i => i.Id == "6176e5d3654d28089f656e25");
            var arhava = _AppState.Products.FirstOrDefault(i => i.Id == "6176e589654d28089f656e21");
            var koishaklach = _AppState.Products.FirstOrDefault(i => i.Id == "6176e645514ed3ae2d40d911");
            var plasticBag = _AppState.Products.FirstOrDefault(i => i.Id == "6176e5f0654d28089f656e29");

            var res = new List<OrderItem>();
            
            items.ForEach(i =>
            {
                if (i.Id is Constants.General.IsraeliSetItemId or Constants.General.YaneverSetItemId)
                {
                    res.AddOrMergeRange(new[]
                    {
                        //lulav.ToOrderItem(0, lulav.Prices.Length - 1, i.Qty + (i.Id is Constants.General.IsraeliSetItemId ? (int)(i.Qty * 0.2) : 0)),
                        //hadas.ToOrderItem(0, hadas.Prices.Length - 1, i.Qty),
                        arhava.ToOrderItem(0, 0, i.Qty),
                        koishaklach.ToOrderItem(0, 0, i.Qty),
                        plasticBag.ToOrderItem(0, 0, i.Qty),
                        new()
                        {
                            Id = i.Id,
                            Name = i.Name.Replace("set", "Esrog", StringComparison.OrdinalIgnoreCase),
                            Option = i.Option,
                            PriceType = i.PriceType,
                            Qty = i.Qty
                        }
                    });
                }
                else
                {
                    res.AddOrMerge(i);
                }

                if (i.Id == Constants.General.YaneverSetItemId && i.Price >= 50)
                {
                    //res.AddOrMerge(hadas.ToOrderItem(0, hadas.Prices.Length - 2, i.Qty));
                }
            });
            
            
            return res;
        }
    }
}
