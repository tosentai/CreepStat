using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using CreepStat.Services;
using CreepStat.Models;
using Microsoft.AspNetCore.Authorization;

namespace CreepStat.Controllers
{
    public class NewsController : Controller
    {
        private readonly INewsService _newsService;

        public NewsController(INewsService newsService)
        {
            _newsService = newsService;
        }

        public async Task<IActionResult> Index()
        {
            var newsList = await _newsService.GetAllNewsAsync();
            return View(newsList);
        }

        public async Task<IActionResult> Details(int id)
        {
            var news = await _newsService.GetNewsByIdAsync(id);
            if (news == null)
            {
                return NotFound();
            }
            return View(news);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(News news)
        {
            if (ModelState.IsValid)
            {
                await _newsService.CreateNewsAsync(news);
                return RedirectToAction(nameof(Index));
            }
            return View(news);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var news = await _newsService.GetNewsByIdAsync(id);
            if (news == null)
            {
                return NotFound();
            }
            return View(news);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, News news)
        {
            if (id != news.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _newsService.UpdateNewsAsync(news);
                return RedirectToAction(nameof(Index));
            }
            return View(news);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var news = await _newsService.GetNewsByIdAsync(id);
            if (news == null)
            {
                return NotFound();
            }

            await _newsService.DeleteNewsAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
