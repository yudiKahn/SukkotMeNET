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
        
        public static string GetFaIcon(this Item item)
        {
            return item.Category switch
            {
                _ => item.Category
            };
        }

        #endregion
    }
}
