namespace SukkotMeNET.Models;

public class Address
{
    public string Street { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string State { get; set; } = string.Empty;
    public int Zip { get; set; }

    public override string ToString() => $"{Street} St {City} {State} {Zip}";
}