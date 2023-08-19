namespace Chinook.Domain.ClientModels;

public class Playlist
{
    public string Name { get; set; }
    public List<PlaylistTrack> Tracks { get; set; }
}