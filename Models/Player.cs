using System.Text.Json.Serialization;

namespace CreepStat.Models
{
    public class PlayerProfile
    {
        public int? solo_competitive_rank { get; set; }
        public int? competitive_rank { get; set; }
        public int? rank_tier { get; set; }
        public int? leaderboard_rank { get; set; }
        public Profile profile { get; set; }
        public List<RecentMatch> RecentMatches { get; set; }
        public List<PlayerPeer> PlayersPlayedWith { get; set; }
        public List<PlayerHero> HeroesPlayed { get; set; }
        public PlayerWinLoss WinLoss { get; set; }
        public PlayerBenchmarks Benchmarks { get; set; }
    }

    public class Profile
    {
        public int account_id { get; set; }
        public string personaname { get; set; }
        public string name { get; set; }
        public bool plus { get; set; }
        public int cheese { get; set; }
        public string steamid { get; set; }
        public string avatar { get; set; }
        public string avatarmedium { get; set; }
        public string avatarfull { get; set; }
        public string profileurl { get; set; }
        public string last_login { get; set; }
        public string loccountrycode { get; set; }
        public bool is_contributor { get; set; }
        public bool is_subscriber { get; set; }
    }

    public class PlayerPeer
    {
        [JsonPropertyName("account_id")]
        public int AccountId { get; set; }

        [JsonPropertyName("personaname")]
        public string PersonaName { get; set; }

        [JsonPropertyName("games")]
        public int Games { get; set; }

        [JsonPropertyName("with_win")]
        public int WithWin { get; set; }

        public double WinRate { get; set; }

        [JsonPropertyName("avatar")]
        public string Avatar { get; set; }
    }

    public class PlayerHero
    {
        [JsonPropertyName("hero_id")]
        public int HeroId { get; set; }
        [JsonPropertyName("games")]
        public int Games { get; set; }
        [JsonPropertyName("win")]
        public int Win { get; set; }
        public double WinRate { get; set; }
        public string HeroName { get; set; }
    }

    public class PlayerWinLoss
    {
        public int Win { get; set; }
        public int Lose { get; set; }
        public double WinRate => Win + Lose > 0 ? (double)Win / (Win + Lose) * 100 : 0;
    }

    public class PlayerBenchmarks
    {
        public double WinRate { get; set; }
        public double AverageKills { get; set; }
        public double AverageDeaths { get; set; }
        public double AverageAssists { get; set; }
        [JsonPropertyName("gold_per_min")]
        public double AverageGoldPerMin { get; set; }
        [JsonPropertyName("xp_per_min")]
        public double AverageXpPerMin { get; set; }
        [JsonPropertyName("last_hits_per_min")]
        public double AverageLastHits { get; set; }
        [JsonPropertyName("hero_damage_per_min")]
        public double AverageHeroDamage { get; set; }
        [JsonPropertyName("hero_healing_per_min")]
        public double AverageHeroHealing { get; set; }
        [JsonPropertyName("tower_damaage")]
        public double AverageTowerDamage { get; set; }
        public double AverageDuration { get; set; }

        public int MaxKills { get; set; }
        public int MaxDeaths { get; set; }
        public int MaxAssists { get; set; }
        public int MaxGoldPerMin { get; set; }
        public int MaxXpPerMin { get; set; }
        public int MaxLastHits { get; set; }
        public int MaxHeroDamage { get; set; }
        public int MaxHeroHealing { get; set; }
        public int MaxTowerDamage { get; set; }
        public int MaxDuration { get; set; }
    }
}
