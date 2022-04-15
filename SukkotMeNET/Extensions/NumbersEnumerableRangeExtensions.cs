namespace SukkotMeNET.Extensions
{
    public static class IEnumerableExtensions
    {
        public static string GetFriednlyRange<T>(this IEnumerable<T> numbers,char currency = '$')
        {
            if (!numbers.Any())
                return string.Empty;
            else if (numbers.Count() == 1)
                return $"{currency}{numbers.ElementAt(0)}";
            return $"{currency}{numbers.ElementAt(0)} - {currency}{numbers.ElementAt(numbers.Count() - 1)}";
        }

    }
}
