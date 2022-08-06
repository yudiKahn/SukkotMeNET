using SukkotMeNET.Models;

namespace SukkotMeNET.Extensions
{
    public static class AlertTypeExtensions
    {
        public static string ToFriendlyString(this AlertType alertType) => alertType switch
        {
            AlertType.Information => "green",
            AlertType.Success => "green",
            _ => "red"
        };
    }
}
