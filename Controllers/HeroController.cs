using CreepStat.Models;
using CreepStat.Services;
using Microsoft.AspNetCore.Mvc;

namespace CreepStat.Controllers
{
    public class HeroController : Controller
    {
        private readonly HeroService _heroService;

        public HeroController(HeroService heroService)
        {
            _heroService = heroService;
        }

        public async Task<IActionResult> Index()
        {
            var heroes = await _heroService.GetHeroesAsync();

            var groupedHeroes = heroes
                .GroupBy(h => h.PrimaryAttr)
                .ToDictionary(g => g.Key, g => g.ToList());

            ViewData["GroupedHeroes"] = groupedHeroes;
            ViewBag.HeroDictionary = new HeroDictionary(heroes).GetDictionary();

            return View(heroes);
        }

        public async Task<IActionResult> GetMatchups(int heroId)
        {
            var matchups = await _heroService.GetHeroMatchupsAsync(heroId);
            var heroes = await _heroService.GetHeroesAsync();
            var heroDictionary = new HeroDictionary(heroes);

            foreach (var matchup in matchups)
            {
                matchup.HeroName = heroDictionary.GetHeroName(matchup.HeroId);
            }

            return Json(matchups);
        }

    }

}
