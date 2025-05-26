using CreepStat.Data;
using CreepStat.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CreepStat.Services
{
    public class FavoriteStreamersService
    {
        private readonly ApplicationDbContext _context;
        private readonly TwitchApiService _twitchService;

        public FavoriteStreamersService(ApplicationDbContext context, TwitchApiService twitchService)
        {
            _context = context;
            _twitchService = twitchService;
        }

        public async Task<bool> AddToFavoritesAsync(string userId, string streamerId)
        {
            var existing = await _context.FavoriteStreamers
                .FirstOrDefaultAsync(fs => fs.UserId == userId && fs.StreamerId == streamerId);

            if (existing != null)
                return false; 

            var streamer = await _twitchService.GetStreamerByIdAsync(streamerId);
            if (streamer == null)
                return false;

            var favorite = new FavoriteStreamer
            {
                UserId = userId,
                StreamerId = streamerId,
                StreamerName = streamer.Username,
                StreamerDisplayName = streamer.DisplayName,
                ProfileImageUrl = streamer.ProfileImageUrl,
                AddedAt = DateTime.UtcNow
            };

            _context.FavoriteStreamers.Add(favorite);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveFromFavoritesAsync(string userId, string streamerId)
        {
            var favorite = await _context.FavoriteStreamers
                .FirstOrDefaultAsync(fs => fs.UserId == userId && fs.StreamerId == streamerId);

            if (favorite == null)
                return false;

            _context.FavoriteStreamers.Remove(favorite);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<FavoriteStreamer>> GetUserFavoritesAsync(string userId)
        {
            return await _context.FavoriteStreamers
                .Where(fs => fs.UserId == userId)
                .OrderByDescending(fs => fs.AddedAt)
                .ToListAsync();
        }

        public async Task<bool> IsStreamerFavoriteAsync(string userId, string streamerId)
        {
            return await _context.FavoriteStreamers
                .AnyAsync(fs => fs.UserId == userId && fs.StreamerId == streamerId);
        }

        public async Task<List<string>> GetUserFavoriteStreamerIdsAsync(string userId)
        {
            return await _context.FavoriteStreamers
                .Where(fs => fs.UserId == userId)
                .Select(fs => fs.StreamerId)
                .ToListAsync();
        }

        public async Task<Dictionary<string, bool>> GetFavoriteStatusForStreamersAsync(string userId, IEnumerable<string> streamerIds)
        {
            var favoriteIds = await _context.FavoriteStreamers
                .Where(fs => fs.UserId == userId && streamerIds.Contains(fs.StreamerId))
                .Select(fs => fs.StreamerId)
                .ToListAsync();

            return streamerIds.ToDictionary(id => id, id => favoriteIds.Contains(id));
        }
    }
}