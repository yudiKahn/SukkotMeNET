﻿using IEsrog.Extensions;
using System.Text.RegularExpressions;
using IEsrog.Models;
using IEsrog.Pages;

namespace IEsrog.Services
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
                htmlValues.GetValueOrDefault(m.Groups[1].Value.Trim(), ""));

            return newHtml;

        }

        public List<OrderItem> GetItemsInTheBox(List<OrderItem> items)
        {
            //todo: match by price in doc. Not current price
            //todo: method invokes few times when requested from UI
            var map = _AppState.Products
                .ToDictionary(k => k.Id);

            var res = new List<OrderItem>(items.Count);
            foreach (var item in items)
            {
                if (item.ProductId is {} pid && map.TryGetValue(pid, out var p) &&
                    p.Includes?.Any() == true)
                {
                    var opt = item.Option;
                    var inc = p.Includes
                        .Select(i => map[i.ProductId].ToModel(opt, item.Qty))
                        .ToArray();
                    
                    res.AddOrMergeRange(inc);
                }
                else
                {
                    res.AddOrMerge(item.Clone());
                }
            }

            return res;
        }
    }
}
