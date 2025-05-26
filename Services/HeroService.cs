using System.Text.Json;
using CreepStat.Models;

namespace CreepStat.Services
{
    public class HeroService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://api.opendota.com/api/heroes";

        public HeroService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Hero>> GetHeroesAsync()
        {
            var response = await _httpClient.GetStringAsync(BaseUrl);
            return JsonSerializer.Deserialize<List<Hero>>(response, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<List<HeroMatchup>> GetHeroMatchupsAsync(int heroId)
        {
            var response = await _httpClient.GetStringAsync($"{BaseUrl}/{heroId}/matchups");
            return JsonSerializer.Deserialize<List<HeroMatchup>>(response, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
    }
}
