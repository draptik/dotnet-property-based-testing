namespace SongRecommendations;

public interface ISongService
{
  Task<IReadOnlyCollection<User>> GetTopListenersAsync(int songId);
  Task<IReadOnlyCollection<Scrobble>> GetTopScrobblesAsync(string userName);
}

public sealed record Song(int Id, bool IsVerifiedArtist, byte Rating);
public sealed record Scrobble(Song Song, int ScrobbleCount);
public sealed record User(string UserName, int TotalScrobbleCount);