using SukkotMeNET.Extensions;
using System.Text.RegularExpressions;

namespace SukkotMeNET.Models
{
    public class InvoiceHelper
    {
        public static string GetInvoiceHTML(Order order, User user)
        {
            var invoicePath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "invoice.html");
            if (File.Exists(invoicePath))
            {
                var html = File.ReadAllText(invoicePath);

                Dictionary<string, string> htmlValues = new()
                {
                    {"OrderId", order.Id },
                    {"Created", order.CreatedAt.ToString("MMMM dd yyyy hh:mm") },
                    {"UserName", $"{user.FirstName} {user.LastName}"  },
                    {"UserEmail", user.Email },
                    {"UserPhone", user.PhoneNumber },
                    {"Total", order.Items.GetTotal().ToString("N2") },
                    {"Items", string.Join(' ',order.Items.Select(item => $@"<tr>
                        <td>{item.Name}</td>
                        <td style={'"'}text-align:right;{'"'}>{item.Qty}</td>
                        <td style={'"'}text-align:right;{'"'}>${item.Price}</td>
                    </tr> ")) }
                };

                var newHtml = Regex.Replace(html, @"\[(.+?)\]", m => htmlValues[m.Groups[1].Value.Trim()]);

                return newHtml;
            }

            return string.Empty;
        }
    }
}
