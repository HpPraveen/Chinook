using Chinook.Domain.Models;

namespace Chinook.Services.Interfaces
{
    public interface IPlayListService
    {
        List<UserPlaylist> GetUserPlaylist(string currentUserId);
        Playlist? GetFavoritePlaylist(string currentUserId);
    }
}
