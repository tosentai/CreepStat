using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CreepStat.Data;
using CreepStat.Models;
using System.Threading.Tasks;
using CreepStat.Services;
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

        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize]
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

        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var news = await _newsService.GetNewsByIdAsync(id);
            if (news == null)
            {
                return NotFound();
            }
            return View(news);
        }

        [Authorize]
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

        [Authorize]
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

        public async Task<IActionResult> Details(int id)
        {
            var news = await _newsService.GetNewsByIdAsync(id);
            if (news == null)
            {
                return NotFound();
            }
            return View(news);
        }
    }
}
