using CreepStat.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CreepStat.Services
{
    public class OpenDotaService
    {
        private readonly HttpClient _httpClient;

        public OpenDotaService(HttpClient httpClient)
        {
            _httpClient = httpClient;
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
            return JsonSerializer.Deserialize<PlayerProfile>(response, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
    }
}
