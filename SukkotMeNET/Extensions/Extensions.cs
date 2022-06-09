using SukkotMeNET.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SukkotMeNET.Extensions
{
    public static class Extensions
    {
        #region Item Extensions

        public static string GetItemIcon(this Item item)
        {
            var name = $"{item.Name.ToLower()} {item.Category.ToLower()}";
            if (name.Contains("lulav"))
                return "/images/lulav.png";
            else if (name.Contains("hadas"))
                return "/images/hadas.png";
            else if (name.Contains("aruvos") || name.Contains("hoshnos"))
                return "/images/harava.png";
            else
                return "/images/esrog.png";
        }

        #endregion
    }
}
