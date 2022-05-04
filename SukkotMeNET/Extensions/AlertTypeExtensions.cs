using SukkotMeNET.Models;

namespace SukkotMeNET.Extensions
{
    public static class AlertTypeExtensions
    {
        public static string ToFriendlyString(this AlertType alertType) => alertType switch
        {
            AlertType.Information => "indigo",
            AlertType.Success => "green",
            _ => "red"
        };
    }
}
