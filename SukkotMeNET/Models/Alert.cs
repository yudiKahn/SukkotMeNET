namespace SukkotMeNET.Models
{
    public record Alert(string Title, string Message, AlertType Type = AlertType.Information);

    public enum AlertType
    {
        Success,
        Information,
        Error,
    }
}
