using Chinook.Domain;
using Chinook.Domain.ClientModels;
using Chinook.Domain.Models;
using Chinook.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Playlist = Chinook.Domain.Models.Playlist;

namespace Chinook.Services
{
    public class TrackService : ITrackService
    {
        private readonly IDbContextFactory<ChinookContext> _dbFactory;

        public TrackService(IDbContextFactory<ChinookContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public List<PlaylistTrack> GetTracksForArtist(long artistId, string currentUserId)
        {
            using var dbContext = _dbFactory.CreateDbContext();

            return dbContext.Tracks.Where(a => a.Album != null && a.Album.ArtistId == artistId)
                .Include(a => a.Album)
                .Select(t => new PlaylistTrack
                {
                    TrackId = t.TrackId,
                    TrackName = t.Name,
                    AlbumTitle = (t.Album == null ? "-" : t.Album.Title),
                    ArtistName = (t.Album == null ? "" : t.Album.Artist.Name),
                    IsFavorite = t.Playlists
                        .Any(p => p.UserPlaylists.Any(up => up.UserId == currentUserId
                                                            && up.Playlist.Name == "My favorite tracks"))
                }).ToList();
        }

        public bool AddTrackToPlayList(long trackId, string playListName, string currentUserId)
        {
            return AddOrRemoveTrack(trackId, playListName, currentUserId, false);
        }

        public bool RemoveTrackFromPlayList(long trackId, string playListName, string currentUserId)
        {
            return AddOrRemoveTrack(trackId, playListName, currentUserId, true);
        }

        public bool AddOrRemoveTrack(long trackId, string playListName, string currentUserId, bool isRemove)
        {
            try
            {
                using var dbContext = _dbFactory.CreateDbContext();

                var existingPlaylist = dbContext.Playlists.Include(p => p.Tracks)
                    .FirstOrDefault(p => p.Name == playListName
                                         && p.UserPlaylists.Any(up => up.UserId == currentUserId));

                var track = dbContext.Tracks.FirstOrDefault(t => t.TrackId == trackId);

                if (track == null) return false;

                if (existingPlaylist == null)
                {
                    // Create a new playlist
                    var newPlaylist = new Playlist
                    {
                        PlaylistId = dbContext.Playlists.OrderBy(a => a.PlaylistId).Last().PlaylistId + 1,
                        Name = playListName,
                        Tracks = new List<Track>(),
                        UserPlaylists = new List<UserPlaylist>()
                    };

                    var userPlaylist = new UserPlaylist()
                    {
                        UserId = currentUserId
                    };

                    newPlaylist.UserPlaylists.Add(userPlaylist);
                    newPlaylist.Tracks.Add(track);

                    dbContext.Playlists.Add(newPlaylist);
                }
                else
                {
                    // Check if the track is in a playlist
                    var existingFavoriteTrack = existingPlaylist?.Tracks?.FirstOrDefault(t => t.TrackId == trackId);

                    if (existingFavoriteTrack == null)
                    {
                        // Add the track to the playlist
                        existingPlaylist?.Tracks?.Add(track);
                    }
                    else
                    {
                        if (isRemove == true)
                        {
                            // Remove the track from the favorite playlist
                            existingPlaylist?.Tracks?.Remove(existingFavoriteTrack);
                        }
                    }
                }

                dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return false;
            }
        }
    }
}
