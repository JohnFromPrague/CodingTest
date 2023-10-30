using System.Text.Json.Serialization;

namespace CleaningRobot.Models
{
    internal record class Position : IComparable<Position>
    {
        [JsonPropertyName("X")]
        public required int X { get; set; }

        [JsonPropertyName("Y")]
        public required int Y { get; set; }

        public int CompareTo(Position? other)
        {
            if (other == null || this.X > other.X)
            {
                return 1;
            }

            if (this.X < other.X || this.Y < other.Y)
            {
                return -1;
            }

            return 0;
        }
    }
}
