using CreepStat.Models;
using CreepStat.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CreepStat.Controllers
{
    public class DistributionController : Controller
    {
        private readonly OpenDotaService _openDotaService;

        public DistributionController(OpenDotaService openDotaService)
        {
            _openDotaService = openDotaService;
        }

        public async Task<IActionResult> Index()
        {
            RankDistribution rankDistribution = await _openDotaService.GetRankDistributionAsync();
            return View(rankDistribution);
        }
    }
}
