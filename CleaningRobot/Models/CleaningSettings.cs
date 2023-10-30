﻿using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace CleaningRobot.Model
{
    internal record CleaningSettings
    {
        /// <summary>
        /// Map with cells which are cleanable, not cleanable or walls in case of null.
        /// </summary>
        [JsonPropertyName("map")]
        public required List<List<MapCell?>> Map { get; set; }

        [JsonPropertyName("start")]
        public required PositionWithDirection Position { get; set; }

        [JsonPropertyName("commands")]
        public required IEnumerable<Command> Commands { get; set; }

        [JsonPropertyName("battery")]
        public required int Battery { get; set; }
    }
}
