using Chinook.Domain;
using Chinook.Domain.Models;
using Chinook.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Chinook.Services
{
    public class PlayListService : IPlayListService
    {
        private readonly IDbContextFactory<ChinookContext> _dbFactory;

        public PlayListService(IDbContextFactory<ChinookContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public List<UserPlaylist> GetUserPlaylist(string currentUserId)
        {
            using var dbContext = _dbFactory.CreateDbContext();

            return dbContext.UserPlaylists.Where(u => u.UserId == currentUserId)
                .Include(a => a.Playlist.Tracks).ToList();
        }

        public Playlist? GetFavoritePlaylist(string currentUserId)
        {
            using var dbContext = _dbFactory.CreateDbContext();

            return dbContext.Playlists.Include(p => p.Tracks)
                .FirstOrDefault(p => p.Name == "My favorite tracks"
                                     && p.UserPlaylists.Any(up => up.UserId == currentUserId));
        }
    }
}
