using CreepStat.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace CreepStat.Controllers
{
    public class StreamsController : Controller
    {
        private readonly TwitchApiService _twitchService;
        private readonly FavoriteStreamersService _favoriteService;

        public StreamsController(TwitchApiService twitchService, FavoriteStreamersService favoriteService)
        {
            _twitchService = twitchService;
            _favoriteService = favoriteService;
        }

        public async Task<IActionResult> Index(string language = "", bool favoritesOnly = false)
        {
            var streams = await _twitchService.GetDotaStreamsAsync();

            if (!string.IsNullOrEmpty(language))
                streams = streams.Where(s => s.Language.Equals(language, StringComparison.OrdinalIgnoreCase)).ToList();

            var languages = streams.Select(s => s.Language)
                                   .Where(l => !string.IsNullOrEmpty(l))
                                   .Distinct()
                                   .OrderBy(l => l)
                                   .ToList();

            ViewBag.Languages = languages;
            ViewBag.SelectedLanguage = language;
            ViewBag.FavoritesOnly = favoritesOnly;

            var streamerIds = streams.Select(s => s.StreamerId).Distinct();
            var streamersDict = await _twitchService.GetStreamersByIdsAsync(streamerIds);

            foreach (var stream in streams)
            {
                if (streamersDict.TryGetValue(stream.StreamerId, out var streamer))
                {
                    stream.StreamerName = streamer.DisplayName ?? streamer.Username ?? "Unknown";
                }
                else
                {
                    stream.StreamerName = "Unknown";
                }
            }

            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var favoriteStatuses = await _favoriteService.GetFavoriteStatusForStreamersAsync(userId, streamerIds);
                ViewBag.FavoriteStatuses = favoriteStatuses;

                if (favoritesOnly)
                {
                    var favoriteStreamerIds = favoriteStatuses.Where(kvp => kvp.Value).Select(kvp => kvp.Key).ToList();
                    streams = streams.Where(s => favoriteStreamerIds.Contains(s.StreamerId)).ToList();
                }
            }

            return View(streams);
        }

        public async Task<IActionResult> Streamer(string id)
        {
            var streamer = await _twitchService.GetStreamerByIdAsync(id);

            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                ViewBag.IsFavorite = await _favoriteService.IsStreamerFavoriteAsync(userId, id);
            }

            return View(streamer);
        }

        [HttpGet]
        public async Task<IActionResult> GetStreamerDetails(string id)
        {
            var streamer = await _twitchService.GetStreamerByIdAsync(id);
            if (streamer == null)
                return NotFound();

            return Json(streamer);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ToggleFavorite(string streamerId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (await _favoriteService.IsStreamerFavoriteAsync(userId, streamerId))
            {
                await _favoriteService.RemoveFromFavoritesAsync(userId, streamerId);
                return Json(new { success = true, isFavorite = false, message = "Removed from favorites" });
            }
            else
            {
                var added = await _favoriteService.AddToFavoritesAsync(userId, streamerId);
                if (added)
                {
                    return Json(new { success = true, isFavorite = true, message = "Added to favorites" });
                }
                else
                {
                    return Json(new { success = false, message = "Failed to add to favorites" });
                }
            }
        }

        [Authorize]
        public async Task<IActionResult> Favorites()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var favorites = await _favoriteService.GetUserFavoritesAsync(userId);
            return View(favorites);
        }
    }
}