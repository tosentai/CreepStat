using System.Text.Json;

namespace CreepStat.Services
{
    public class TwitchAuthHelper
    {
        private readonly IConfiguration _configuration;
        public TwitchAuthHelper(IConfiguration configuration) => _configuration = configuration;

        public async Task<string> GetAccessTokenAsync()
        {
            using var client = new HttpClient();
            var values = new Dictionary<string, string>
            {
                ["client_id"] = _configuration["TwitchApi:ClientId"],
                ["client_secret"] = _configuration["TwitchApi:ClientSecret"],
                ["grant_type"] = "client_credentials"
            };

            var content = new FormUrlEncodedContent(values);
            var response = await client.PostAsync("https://id.twitch.tv/oauth2/token", content);
            var result = await response.Content.ReadFromJsonAsync<JsonElement>();

            return result.GetProperty("access_token").GetString();
        }
    }
}
