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

public class AuthController : Controller
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ApplicationDbContext _context;
    private readonly List<string> _adminSteamIds = new() {"76561199231604313", "76561198424694488" }; 

    public AuthController(IHttpClientFactory httpClientFactory, IConfiguration configuration, ApplicationDbContext context)
    {
        _httpClient = httpClientFactory.CreateClient();
        _configuration = configuration;
        _context = context;
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
        var players = root.GetProperty("response").GetProperty("players");
        if (players.GetArrayLength() == 0)
            return Redirect("/");

        var player = players[0];
        var steamName = player.GetProperty("personaname").GetString();
        var avatarUrl = player.GetProperty("avatarfull").GetString();

        string role = _adminSteamIds.Contains(steamId) ? "Admin" : "User";

        var user = await _context.Users.Include(u => u.LoginHistories).FirstOrDefaultAsync(u => u.SteamID == steamId);
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
            LoginTime = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();

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



    public IActionResult Logout()
    {
        return SignOut(new AuthenticationProperties { RedirectUri = "/" },
            CookieAuthenticationDefaults.AuthenticationScheme);
    }
}