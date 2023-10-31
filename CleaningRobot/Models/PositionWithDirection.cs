using System.Text.Json.Serialization;

namespace CleaningRobot.Models
{
    internal record PositionWithDirection : Position
    {
        [JsonPropertyName("facing")]
        public Direction Direction { get; set; }
    }
}
