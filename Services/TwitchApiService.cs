using CreepStat.Models;
using System.Text.Json;
using Stream = CreepStat.Models.Stream;

namespace CreepStat.Services
{
    public class TwitchApiService
    {
        private readonly TwitchAuthHelper _authHelper;
        private readonly IConfiguration _configuration;
        private string _accessToken;

        public TwitchApiService(TwitchAuthHelper authHelper, IConfiguration configuration)
        {
            _authHelper = authHelper;
            _configuration = configuration;
        }

        private async Task EnsureAccessTokenAsync()
        {
            if (string.IsNullOrEmpty(_accessToken))
                _accessToken = await _authHelper.GetAccessTokenAsync();
        }

        public async Task<List<Stream>> GetDotaStreamsAsync()
        {
            await EnsureAccessTokenAsync();

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Client-ID", _configuration["TwitchApi:ClientId"]);
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_accessToken}");

            var response = await client.GetFromJsonAsync<JsonElement>("https://api.twitch.tv/helix/streams?game_id=29595");

            var streams = new List<Stream>();
            foreach (var item in response.GetProperty("data").EnumerateArray())
            {
                streams.Add(new Stream
                {
                    Id = item.GetProperty("id").GetString(),
                    Title = item.GetProperty("title").GetString(),
                    ThumbnailUrl = item.GetProperty("thumbnail_url").GetString(),
                    ViewerCount = item.GetProperty("viewer_count").GetInt32(),
                    StreamerId = item.GetProperty("user_id").GetString(),
                    Language = item.GetProperty("language").GetString()
                });
            }
            return streams;
        }

        public async Task<Streamer> GetStreamerByIdAsync(string id)
        {
            await EnsureAccessTokenAsync();

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Client-ID", _configuration["TwitchApi:ClientId"]);
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_accessToken}");

            var response = await client.GetFromJsonAsync<JsonElement>($"https://api.twitch.tv/helix/users?id={id}");
            var user = response.GetProperty("data")[0];

            return new Streamer
            {
                Id = user.GetProperty("id").GetString(),
                Username = user.GetProperty("login").GetString(),
                DisplayName = user.GetProperty("display_name").GetString(),
                ProfileImageUrl = user.GetProperty("profile_image_url").GetString(),
                Description = user.GetProperty("description").GetString()
            };
        }

        public async Task<Dictionary<string, Streamer>> GetStreamersByIdsAsync(IEnumerable<string> ids)
        {
            await EnsureAccessTokenAsync();

            var idsQuery = string.Join("&id=", ids.Distinct());
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Client-ID", _configuration["TwitchApi:ClientId"]);
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_accessToken}");

            var response = await client.GetFromJsonAsync<JsonElement>($"https://api.twitch.tv/helix/users?id={idsQuery}");

            var streamers = new Dictionary<string, Streamer>();

            foreach (var user in response.GetProperty("data").EnumerateArray())
            {
                var streamer = new Streamer
                {
                    Id = user.GetProperty("id").GetString(),
                    Username = user.GetProperty("login").GetString(),
                    DisplayName = user.GetProperty("display_name").GetString(),
                    ProfileImageUrl = user.GetProperty("profile_image_url").GetString(),
                    Description = user.GetProperty("description").GetString()
                };

                streamers[streamer.Id] = streamer;
            }

            return streamers;
        }

    }
}
