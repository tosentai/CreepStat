using CreepStat.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CreepStat.Services
{
    public class OpenDotaService
    {
        private readonly HttpClient _httpClient;

        public OpenDotaService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        string GetHeroName(int heroId)
    {
        var heroNames = new Dictionary<int, string>
        {
            { 1, "antimage" },
            { 2, "axe" },
            { 3, "bane" },
            { 4, "bloodseeker" },
            { 5, "crystal_maiden" },
            { 6, "drow_ranger" },
            { 7, "earthshaker" },
            { 8, "juggernaut" },
            { 9, "mirana" },
            { 10, "morphling" },
            { 11, "nevermore" },
            { 12, "phantom_lancer" },
            { 13, "puck" },
            { 14, "pudge" },
            { 15, "razor" },
            { 16, "sand_king" },
            { 17, "storm_spirit" },
            { 18, "sven" },
            { 19, "tiny" },
            { 20, "vengefulspirit" },
            { 21, "windrunner" },
            { 22, "zuus" },
            { 23, "kunkka" },
            { 25, "lina" },
            { 26, "lion" },
            { 27, "shadow_shaman" },
            { 28, "slardar" },
            { 29, "tidehunter" },
            { 30, "witch_doctor" },
            { 31, "lich" },
            { 32, "riki" },
            { 33, "enigma" },
            { 34, "tinker" },
            { 35, "sniper" },
            { 36, "necrolyte" },
            { 37, "warlock" },
            { 38, "beastmaster" },
            { 39, "queenofpain" },
            { 40, "venomancer" },
            { 41, "faceless_void" },
            { 42, "skeleton_king" },
            { 43, "death_prophet" },
            { 44, "phantom_assassin" },
            { 45, "pugna" },
            { 46, "templar_assassin" },
            { 47, "viper" },
            { 48, "luna" },
            { 49, "dragon_knight" },
            { 50, "dazzle" },
            { 51, "rattletrap" },
            { 52, "leshrac" },
            { 53, "furion" },
            { 54, "life_stealer" },
            { 55, "dark_seer" },
            { 56, "clinkz" },
            { 57, "omniknight" },
            { 58, "enchantress" },
            { 59, "huskar" },
            { 60, "night_stalker" },
            { 61, "broodmother" },
            { 62, "bounty_hunter" },
            { 63, "weaver" },
            { 64, "jakiro" },
            { 65, "batrider" },
            { 66, "chen" },
            { 67, "spectre" },
            { 68, "ancient_apparition" },
            { 69, "doom_bringer" },
            { 70, "ursa" },
            { 71, "spirit_breaker" },
            { 72, "gyrocopter" },
            { 73, "alchemist" },
            { 74, "invoker" },
            { 75, "silencer" },
            { 76, "obsidian_destroyer" },
            { 77, "lycan" },
            { 78, "brewmaster" },
            { 79, "shadow_demon" },
            { 80, "lone_druid" },
            { 81, "chaos_knight" },
            { 82, "meepo" },
            { 83, "treant" },
            { 84, "ogre_magi" },
            { 85, "undying" },
            { 86, "rubick" },
            { 87, "disruptor" },
            { 88, "nyx_assassin" },
            { 89, "naga_siren" },
            { 90, "keeper_of_the_light" },
            { 91, "wisp" },
            { 92, "visage" },
            { 93, "slark" },
            { 94, "medusa" },
            { 95, "troll_warlord" },
            { 96, "centaur" },
            { 97, "magnataur" },
            { 98, "shredder" },
            { 99, "bristleback" },
            { 100, "tusk" },
            { 101, "skywrath_mage" },
            { 102, "abaddon" },
            { 103, "elder_titan" },
            { 104, "legion_commander" },
            { 105, "techies" },
            { 106, "ember_spirit" },
            { 107, "earth_spirit" },
            { 108, "abyssal_underlord" },
            { 109, "terrorblade" },
            { 110, "phoenix" },
            { 111, "oracle" },
            { 112, "winter_wyvern" },
            { 113, "arc_warden" },
            { 114, "monkey_king" },
            { 119, "dark_willow" },
            { 120, "pangolier" },
            { 121, "grimstroke" },
            { 123, "hoodwink" },
            { 126, "void_spirit" },
            { 128, "snapfire" },
            { 129, "mars" },
            { 135, "dawnbreaker" },
            { 136, "marci" },
            { 137, "primal_beast" },
            { 138, "muerta" }
        };
        return heroNames.ContainsKey(heroId) ? heroNames[heroId] : "unknown";
    }

        public async Task<List<Team>> GetTeamsAsync()
        {
            string url = "https://api.opendota.com/api/teams";
            var response = await _httpClient.GetStringAsync(url);
            return JsonSerializer.Deserialize<List<Team>>(response, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<RankDistribution> GetRankDistributionAsync()
        {
            string url = "https://api.opendota.com/api/distributions";
            var response = await _httpClient.GetStringAsync(url);
            return JsonSerializer.Deserialize<RankDistribution>(response, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<PlayerProfile> GetPlayerProfileAsync(int accountId)
        {
            string url = $"https://api.opendota.com/api/players/{accountId}";
            var response = await _httpClient.GetStringAsync(url);
            var playerProfile = JsonSerializer.Deserialize<PlayerProfile>(response, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            playerProfile.RecentMatches = (await GetRecentMatchesAsync(accountId)).Take(20).ToList();

            playerProfile.PlayersPlayedWith = (await GetPlayersPlayedWithAsync(accountId))
            .OrderByDescending(p => p.Games)
            .Take(5)
            .ToList();

            playerProfile.HeroesPlayed = (await GetHeroesPlayedAsync(accountId))
            .OrderByDescending(h => h.Games)
            .Take(10)
            .ToList();

            playerProfile.WinLoss = await GetPlayerWinLossAsync(accountId);

            playerProfile.Benchmarks = await GetPlayerBenchmarksAsync(playerProfile.RecentMatches);

            return playerProfile;
        }

        public async Task<List<RecentMatch>> GetRecentMatchesAsync(int accountId)
        {
            string url = $"https://api.opendota.com/api/players/{accountId}/matches";
            var response = await _httpClient.GetStringAsync(url);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            return JsonSerializer.Deserialize<List<RecentMatch>>(response, options);
        }


        public async Task<List<PlayerPeer>> GetPlayersPlayedWithAsync(int accountId)
        {
            string url = $"https://api.opendota.com/api/players/{accountId}/peers";
            var response = await _httpClient.GetStringAsync(url);
            var peers = JsonSerializer.Deserialize<List<PlayerPeer>>(response, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            foreach (var peer in peers)
            {
                peer.WinRate = peer.Games > 0 ? (double)peer.WithWin / peer.Games * 100 : 0;
            }

            return peers;
        }

        public async Task<List<PlayerHero>> GetHeroesPlayedAsync(int accountId)
        {
            string url = $"https://api.opendota.com/api/players/{accountId}/heroes";
            var response = await _httpClient.GetStringAsync(url);
            var heroes = JsonSerializer.Deserialize<List<PlayerHero>>(response, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            foreach (var hero in heroes)
            {
                hero.WinRate = hero.Games > 0 ? (double)hero.Win / hero.Games * 100 : 0;
                hero.HeroName = GetHeroName(hero.HeroId);
            }

            return heroes;
        }

        public async Task<PlayerWinLoss> GetPlayerWinLossAsync(int accountId)
        {
            string url = $"https://api.opendota.com/api/players/{accountId}/wl";
            var response = await _httpClient.GetStringAsync(url);
            return JsonSerializer.Deserialize<PlayerWinLoss>(response, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<PlayerBenchmarks> GetPlayerBenchmarksAsync(List<RecentMatch> recentMatches)
        {
            var benchmarks = new PlayerBenchmarks();

            if (recentMatches == null || recentMatches.Count == 0)
                return benchmarks;

            int wins = recentMatches.Count(m => m.Won);
            benchmarks.WinRate = (double)wins / recentMatches.Count * 100;

            benchmarks.AverageKills = recentMatches.Average(m => m.Kills);
            benchmarks.AverageDeaths = recentMatches.Average(m => m.Deaths);
            benchmarks.AverageAssists = recentMatches.Average(m => m.Assists);
            benchmarks.AverageDuration = recentMatches.Average(m => m.Duration);

            benchmarks.MaxKills = recentMatches.Max(m => m.Kills);
            benchmarks.MaxDeaths = recentMatches.Max(m => m.Deaths);
            benchmarks.MaxAssists = recentMatches.Max(m => m.Assists);
            benchmarks.MaxDuration = recentMatches.Max(m => m.Duration);

            benchmarks.AverageGoldPerMin = 0;
            benchmarks.AverageXpPerMin = 0;
            benchmarks.AverageLastHits = 0;
            benchmarks.AverageHeroDamage = 0;
            benchmarks.AverageHeroHealing = 0;
            benchmarks.AverageTowerDamage = 0;

            benchmarks.MaxGoldPerMin = 0;
            benchmarks.MaxXpPerMin = 0;
            benchmarks.MaxLastHits = 0;
            benchmarks.MaxHeroDamage = 0;
            benchmarks.MaxHeroHealing = 0;
            benchmarks.MaxTowerDamage = 0;

            return benchmarks;
        }

        public class Benchmark
        {
            [JsonPropertyName("percentile")]
            public double Percentile { get; set; }

            [JsonPropertyName("value")]
            public int Value { get; set; }
        }
    }
}
