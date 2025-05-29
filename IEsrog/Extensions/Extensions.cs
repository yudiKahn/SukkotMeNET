using IEsrog.Models;

namespace IEsrog.Extensions
{
    public static class Extensions
    {
        #region Product Extensions

        public static string GetItemIcon(this Product product)
        {

            var name = product.Name.ToLower();
            if (name.Contains("lulav"))
                return "/images/lulav.png";
            if (name.Contains("hadas"))
                return "/images/hadas.png";
            if (name.Contains("aruvos") || name.Contains("hoshnos"))
                return "/images/harava.png";
            if (name.Contains("set"))
                return "/images/set.png";
            if (name.Contains("schach"))
                return "/images/schach.png";
            if (name.Contains("koisaklach"))
                return "/images/koishklach.png";
            if (name.Contains("sukkah"))
                return "/images/sukkah.png";
            if (name.Contains("ring"))
                return "/images/ring.png";

            return "/images/esrog.png";
        }

        #endregion


        public static DateTime ToCaTime(this DateTime dt)
        {
            var res = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(dt, "Pacific Standard Time");
            return res;
        }
        
        
        public static T Clone<T>(this T obj) where T : class, new()
        {
            var res = new T();

            foreach (var prop in obj.GetType().GetProperties())
            {
                prop.SetValue(res, prop.GetValue(obj));
            }

            return res;
        }

        public static Order Clone(this Order o)
        {
            return new Order()
            {
                Comment = o.Comment,
                CreatedAt = o.CreatedAt,
                Id = o.Id,
                IsPacked = o.IsPacked,
                IsPaid = o.IsPaid,
                IsShipped = o.IsShipped,
                ShippingCost = o.ShippingCost,
                UserId = o.UserId,
                Items = o.Items.Select(i => i.Clone()).ToList(),
                Recipient = o.Recipient
            };
        }

        public static User Clone(this User o)
        {
            return new User
            {
                Id = o.Id,
                FirstName = o.FirstName,
                LastName = o.LastName,
                Email = o.Email,
                PhoneNumber = o.PhoneNumber,
                Password = o.Password,
                IsAdmin = o.IsAdmin,
                Address = o.Address.Clone()
            };
        }

        public static string? StrOrNull(this string? str) => string.IsNullOrWhiteSpace(str) ? null : str;
    }
}
