using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

public class AuthController : Controller
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public AuthController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClient = httpClientFactory.CreateClient();
        _configuration = configuration;
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
            var authenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (!authenticateResult.Succeeded)
                return Redirect("/");

            var claims = new List<Claim>(authenticateResult.Principal.Claims);

            var steamIdClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (steamIdClaim == null)
                return Redirect("/");

            var steamId = steamIdClaim.Replace("https://steamcommunity.com/openid/id/", "");

            var apiUrl = $"https://api.steampowered.com/ISteamUser/GetPlayerSummaries/v2/?key={_configuration["Steam:SteamAPIKey"]}&steamids={steamId}";
            var response = await _httpClient.GetStringAsync(apiUrl);
            using var jsonDoc = JsonDocument.Parse(response);
            var root = jsonDoc.RootElement;
            var player = root.GetProperty("response").GetProperty("players")[0];

            var steamName = player.GetProperty("personaname").GetString();
            var avatarUrl = player.GetProperty("avatarfull").GetString();

            claims.Add(new Claim(ClaimTypes.Name, steamName));
            claims.Add(new Claim("Avatar", avatarUrl));
            claims.Add(new Claim("SteamID", steamId));

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties();

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return Redirect("/");
        }


    public IActionResult Logout()
    {
        return SignOut(new AuthenticationProperties { RedirectUri = "/" },
            CookieAuthenticationDefaults.AuthenticationScheme);
    }
}