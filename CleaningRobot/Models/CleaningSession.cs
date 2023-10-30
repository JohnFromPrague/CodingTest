using CleaningRobot.Models;

using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

[assembly: InternalsVisibleTo("CleaningRobot.Tests")]

namespace CleaningRobot.Model
{
    internal record CleaningSession
    {
        /// <summary>
        /// All cells visited.
        /// </summary>
        [JsonPropertyName("visited")]
        public SortedSet<Position> Visited { get; set; } = new SortedSet<Position>();

        /// <summary>
        /// All cells cleaned.
        /// </summary>
        [JsonPropertyName("cleaned")]
        public SortedSet<Position> Cleaned { get; set; } = new SortedSet<Position>();

        /// <summary>
        /// Final position of the robot.
        /// </summary>
        [JsonPropertyName("final")]
        public required PositionWithDirection Position { get; set; }

        /// <summary>
        ///  Final battery left.
        /// </summary>
        [JsonPropertyName("battery")]
        public required int Battery { get; set; }
    }
}
