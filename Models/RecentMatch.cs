using System.Text.Json.Serialization;

namespace CreepStat.Models
{
    public class RecentMatch
    {
        [JsonPropertyName("match_id")]
        public long MatchId { get; set; }

        [JsonPropertyName("player_slot")]
        public int PlayerSlot { get; set; }

        [JsonPropertyName("radiant_win")]
        public bool RadiantWin { get; set; }

        [JsonPropertyName("duration")]
        public int Duration { get; set; }

        [JsonPropertyName("game_mode")]
        public int GameMode { get; set; }

        [JsonPropertyName("hero_id")]
        public int HeroId { get; set; }

        [JsonPropertyName("kills")]
        public int Kills { get; set; }

        [JsonPropertyName("deaths")]
        public int Deaths { get; set; }

        [JsonPropertyName("assists")]
        public int Assists { get; set; }
        public bool Won => (PlayerSlot < 128) == RadiantWin;
    }
}