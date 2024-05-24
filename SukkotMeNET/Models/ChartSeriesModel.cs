using System.Numerics;
using System.Text.Json.Serialization;

namespace SukkotMeNET.Models;

public class ChartSeriesModel
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }
    
    [JsonPropertyName("color")]
    public required string Color { get; set; }
    
    [JsonPropertyName("data")]
    public required List<ChartVector2d> Data { get; set; }
}

public struct ChartVector2d(string x, double y)
{
    [JsonPropertyName("x")]
    public string X { get; set; } = x;

    [JsonPropertyName("y")]
    public double Y { get; set; } = y;
}

/*
 *
 * new[]
   {
       new
       {
           name = "Organic",
           color = "#1A56DB",
           data = new[]
           {
               new { x = "Mon", y = 231 },
               new { x = "Tue", y = 122 },
               new { x = "Wed", y = 63 },
               new { x = "Thu", y = 421 },
               new { x = "Fri", y = 122 },
               new { x = "Sat", y = 323 },
               new { x = "Sun", y = 111 }
           }
       },
       new
       {
           name = "Social media",
           color = "#FDBA8C",
           data = new[]
           {
               new { x = "Mon", y = 232 },
               new { x = "Tue", y = 113 },
               new { x = "Wed", y = 341 },
               new { x = "Thu", y = 224 },
               new { x = "Fri", y = 522 },
               new { x = "Sat", y = 411 },
               new { x = "Sun", y = 243 }
           }
       }
   },
 */