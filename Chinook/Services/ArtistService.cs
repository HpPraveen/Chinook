using Chinook.Domain;
using Chinook.Domain.Models;
using Chinook.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Chinook.Services
{
    public class ArtistService : IArtistService
    {
        private readonly IDbContextFactory<ChinookContext> _dbFactory;

        public ArtistService(IDbContextFactory<ChinookContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public async Task<List<Artist>> GetAllArtists()
        {
            var dbContext = await _dbFactory.CreateDbContextAsync();
            return dbContext.Artists.Include(a => a.Albums).ToList();
        }

        public async Task<Artist> GetArtistById(long artistId)
        {
            await using var dbContext = await _dbFactory.CreateDbContextAsync();
            return await dbContext.Artists.SingleOrDefaultAsync(a => a.ArtistId == artistId)
                   ?? throw new InvalidOperationException();
        }
        public async Task<List<Artist>> SearchArtists(string searchTerm)
        {
            await using var dbContext = await _dbFactory.CreateDbContextAsync();
            var artists = dbContext.Artists
                .Where(a => a.Name != null && a.Name.ToLower().Contains(searchTerm.ToLower()))
                .Include(a => a.Albums).ToList();
            return artists;
        }
    }
}
