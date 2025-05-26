﻿using System.Text.Json.Serialization;

namespace CreepStat.Models
{
    public class Team
    {
        [JsonPropertyName("team_id")]
        public long TeamId { get; set; }

        [JsonPropertyName("rating")]
        public double Rating { get; set; }

        [JsonPropertyName("wins")]
        public int Wins { get; set; }

        [JsonPropertyName("losses")]
        public int Losses { get; set; }

        [JsonPropertyName("last_match_time")]
        public long LastMatchTime { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("tag")]
        public string Tag { get; set; }

        [JsonPropertyName("logo_url")]
        public string LogoUrl { get; set; }
    }
}
