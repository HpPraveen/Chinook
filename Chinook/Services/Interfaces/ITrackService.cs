using Chinook.Domain.ClientModels;

namespace Chinook.Services.Interfaces
{
    public interface ITrackService
    {
        event Action PlayListUpdated;
        List<PlaylistTrack> GetTracksForArtist(long artistId, string currentUserId);
        bool AddTrackToPlayList(long trackId, string playListName, string currentUserId);
        bool RemoveTrackFromPlayList(long trackId, string playListName, string currentUserId);
    }
}
