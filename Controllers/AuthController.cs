using CreepStat.Data;
using CreepStat.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net;

public class AuthController : Controller
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AuthController> _logger;
    private readonly HashSet<string> _adminSteamIds;

    public AuthController(IHttpClientFactory httpClientFactory, IConfiguration configuration,
        ApplicationDbContext context, ILogger<AuthController> logger)
    {
        _httpClient = httpClientFactory.CreateClient();
        _configuration = configuration;
        _context = context;
        _logger = logger;

        _adminSteamIds = _configuration.GetSection("AdminID")
            .GetChildren()
            .Select(x => x.Value)
            .Where(v => !string.IsNullOrEmpty(v))
            .ToHashSet();
    }

    public IActionResult Login()
    {
        var properties = new AuthenticationProperties
        {
            RedirectUri = "/Auth/SteamResponse"
        };
        return Challenge(properties, "Steam");
    }

    public async Task<IActionResult> SteamResponse()
    {
        try
        {
            var authenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (!authenticateResult.Succeeded)
            {
                _logger.LogWarning("Steam authentication failed");
                return Redirect("/?error=auth_failed");
            }

            var claims = new List<Claim>(authenticateResult.Principal.Claims);
            var steamIdClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (steamIdClaim == null)
            {
                _logger.LogWarning("Steam ID claim not found");
                return Redirect("/?error=no_steamid");
            }

            var steamId = steamIdClaim.Replace("https://steamcommunity.com/openid/id/", "");

            var playerData = await GetPlayerDataWithRetry(steamId);

            string steamName = "Unknown User";
            string avatarUrl = "/images/default-avatar.png"; 

            if (playerData != null)
            {
                steamName = playerData.Value.name;
                avatarUrl = playerData.Value.avatar;
            }
            else
            {
                _logger.LogWarning($"Could not retrieve player data for Steam ID: {steamId}");
            }

            string role = _adminSteamIds.Contains(steamId) ? "Admin" : "User";

            await UpdateUserInDatabase(steamId, steamName, avatarUrl, role);

            claims.Add(new Claim(ClaimTypes.Name, steamName));
            claims.Add(new Claim("Avatar", avatarUrl));
            claims.Add(new Claim("SteamID", steamId));
            claims.Add(new Claim(ClaimTypes.Role, role));

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties();

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return Redirect("/");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in SteamResponse method");
            return Redirect("/?error=login_error");
        }
    }

    private async Task<(string name, string avatar)?> GetPlayerDataWithRetry(string steamId, int maxRetries = 3)
    {
        var apiUrl = $"https://api.steampowered.com/ISteamUser/GetPlayerSummaries/v2/?key={_configuration["Steam:SteamAPIKey"]}&steamids={steamId}";

        for (int attempt = 0; attempt < maxRetries; attempt++)
        {
            try
            {
                _logger.LogInformation($"Attempting to get player data, attempt {attempt + 1}");

                var response = await _httpClient.GetAsync(apiUrl);

                if (response.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    _logger.LogWarning($"Rate limited on attempt {attempt + 1}");

                    if (attempt < maxRetries - 1)
                    {
                        var delay = TimeSpan.FromSeconds(Math.Pow(2, attempt));
                        _logger.LogInformation($"Waiting {delay.TotalSeconds} seconds before retry");
                        await Task.Delay(delay);
                        continue;
                    }
                    else
                    {
                        _logger.LogError("Max retries reached, still rate limited");
                        return null;
                    }
                }

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"Steam API returned {response.StatusCode}: {response.ReasonPhrase}");
                    return null;
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                return ParsePlayerData(responseContent);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"HTTP error on attempt {attempt + 1}");

                if (attempt < maxRetries - 1)
                {
                    var delay = TimeSpan.FromSeconds(Math.Pow(2, attempt));
                    await Task.Delay(delay);
                    continue;
                }

                _logger.LogError("Max retries reached, giving up");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error on attempt {attempt + 1}");
                return null;
            }
        }

        return null;
    }

    private (string name, string avatar)? ParsePlayerData(string responseContent)
    {
        try
        {
            using var jsonDoc = JsonDocument.Parse(responseContent);
            var root = jsonDoc.RootElement;
            var players = root.GetProperty("response").GetProperty("players");

            if (players.GetArrayLength() == 0)
            {
                _logger.LogWarning("No players found in Steam API response");
                return null;
            }

            var player = players[0];
            var steamName = player.GetProperty("personaname").GetString() ?? "Unknown User";
            var avatarUrl = player.GetProperty("avatarfull").GetString() ?? "/images/default-avatar.png";

            return (steamName, avatarUrl);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing Steam API response");
            return null;
        }
    }

    private async Task UpdateUserInDatabase(string steamId, string steamName, string avatarUrl, string role)
    {
        try
        {
            var user = await _context.Users.Include(u => u.LoginHistories)
                .FirstOrDefaultAsync(u => u.SteamID == steamId);

            if (user == null)
            {
                user = new User
                {
                    SteamID = steamId,
                    Name = steamName,
                    AvatarUrl = avatarUrl,
                    Role = role
                };
                _context.Users.Add(user);
            }
            else
            {
                user.Name = steamName;
                user.AvatarUrl = avatarUrl;
                user.Role = role;
                _context.Users.Update(user);
            }

            _context.LoginHistories.Add(new LoginHistory
            {
                SteamID = steamId,
                LoginTime = DateTime.Now
            });

            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user in database");
        }
    }

    public IActionResult Logout()
    {
        return SignOut(new AuthenticationProperties { RedirectUri = "/" },
            CookieAuthenticationDefaults.AuthenticationScheme);
    }
}