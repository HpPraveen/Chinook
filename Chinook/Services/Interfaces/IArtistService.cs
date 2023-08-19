using Chinook.Domain.Models;

namespace Chinook.Services.Interfaces
{
    public interface IArtistService
    {
        Task<List<Artist>> GetAllArtists();
        Task<Artist> GetArtistById(long artistId);
        Task<List<Artist>> SearchArtists(string searchTerm);
    }
}
