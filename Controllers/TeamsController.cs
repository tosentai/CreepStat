using CreepStat.Models;
using CreepStat.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CreepStat.Controllers
{
    public class TeamsController : Controller
    {
        private readonly OpenDotaService _openDotaService;

        public TeamsController(OpenDotaService openDotaService)
        {
            _openDotaService = openDotaService;
        }

        public async Task<IActionResult> Index()
        {
            List<Team> teams = await _openDotaService.GetTeamsAsync();

            var filteredTeams = teams.Where(t => !string.IsNullOrEmpty(t.Name) && !string.IsNullOrEmpty(t.Tag) && t.Wins >= 80).ToList();
            var limitedTeams = filteredTeams.Take(50).ToList();
            return View(limitedTeams);
        }
    }
}
