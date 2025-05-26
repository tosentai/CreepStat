using CreepStat.Data;
using CreepStat.Models;
using Microsoft.EntityFrameworkCore;

namespace CreepStat.Services
{
    public class NewsService : INewsService
    {
        private readonly ApplicationDbContext _context;

        public NewsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<News>> GetAllNewsAsync()
        {
            return await _context.News.ToListAsync();
        }

        public async Task<News> GetNewsByIdAsync(int id)
        {
            return await _context.News.FindAsync(id);
        }

        public async Task CreateNewsAsync(News news)
        {
            _context.Add(news);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateNewsAsync(News news)
        {
            _context.Update(news);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteNewsAsync(int id)
        {
            var news = await _context.News.FindAsync(id);
            if (news != null)
            {
                _context.News.Remove(news);
                await _context.SaveChangesAsync();
            }
        }
    }
}