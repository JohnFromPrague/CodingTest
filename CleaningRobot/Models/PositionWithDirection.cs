using CleaningRobot.Models;

using System.Text.Json.Serialization;

namespace CleaningRobot.Model
{
    internal record PositionWithDirection : Position
    {
        [JsonPropertyName("facing")]
        public Direction Direction { get; set; }
    }
}
