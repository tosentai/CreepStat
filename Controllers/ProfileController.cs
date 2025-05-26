using CreepStat.Models;
using CreepStat.Services;
using Microsoft.AspNetCore.Mvc;

namespace CreepStat.Controllers
{
    public class ProfileController : Controller
    {
        private readonly OpenDotaService _openDotaService;

        public ProfileController(OpenDotaService openDotaService)
        {
            _openDotaService = openDotaService;
        }

        public async Task<IActionResult> MyProfile()
        {
            var steamIdClaim = User.FindFirst("SteamID")?.Value;
            if (string.IsNullOrEmpty(steamIdClaim))
            {
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                var steamId = long.Parse(steamIdClaim);
                var accountId = steamId - 76561197960265728;

                var playerProfile = await _openDotaService.GetPlayerProfileAsync((int)accountId);
                return View(playerProfile);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Failed to load profile: {ex.Message}";
                return View("Error");
            }
        }

        [HttpPost]
        public IActionResult SubmitAccountId(int accountId)
        {
            return RedirectToAction("MyProfile", new { accountId });
        }
    }
}