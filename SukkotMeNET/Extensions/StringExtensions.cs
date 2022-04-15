using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SukkotMeNET.Extensions
{
    public static class StringExtensions
    {
        public static bool IsNullOrBlank(this string str) => 
            string.IsNullOrEmpty(str) || string.IsNullOrWhiteSpace(str);
    }
}
