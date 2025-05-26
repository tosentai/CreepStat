using CreepStat.Models;

namespace CreepStat.Services
{
    public interface INewsService
    {
        Task<List<News>> GetAllNewsAsync();
        Task<News> GetNewsByIdAsync(int id);
        Task CreateNewsAsync(News news);
        Task UpdateNewsAsync(News news);
        Task DeleteNewsAsync(int id);
    }
}
