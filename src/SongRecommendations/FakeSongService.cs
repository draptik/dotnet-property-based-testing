using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace SongRecommendations;

[SuppressMessage("Style", "IDE0058:Expression value is never used")]
public class FakeSongService : ISongService
{
  private readonly ConcurrentDictionary<int, Song> _songs = new();
  private readonly ConcurrentDictionary<string, ConcurrentDictionary<int, int>> _users = new();

  public Task<IReadOnlyCollection<User>> GetTopListenersAsync(int songId)
  {
    var listeners =
      from kvp in _users
      where kvp.Value.ContainsKey(songId)
      select new User(kvp.Key, kvp.Value.Values.Sum());

    return Task.FromResult<IReadOnlyCollection<User>>(listeners.ToList());
  }

  public Task<IReadOnlyCollection<Scrobble>> GetTopScrobblesAsync(
    string userName)
  {
    var scrobbles = _users
      .GetOrAdd(userName, new ConcurrentDictionary<int, int>())
      .Select(kvp => new Scrobble(_songs[kvp.Key], kvp.Value));

    return Task.FromResult<IReadOnlyCollection<Scrobble>>(scrobbles.ToList());
  }

  // "Backdoor" for testing
  [SuppressMessage("ReSharper", "UnusedMember.Global")]
  public void Scrobble(string userName, Song song, int scrobbleCount)
  {
    _users.AddOrUpdate(
      userName,
      new ConcurrentDictionary<int, int>(
        [KeyValuePair.Create(song.Id, scrobbleCount)]),
      (_, scrobbles) => AddScrobbles(scrobbles, song, scrobbleCount));

    _songs.AddOrUpdate(song.Id, song, (_, _) => song);
  }

  private static ConcurrentDictionary<int, int> AddScrobbles(
    ConcurrentDictionary<int, int> scrobbles,
    Song song,
    int scrobbleCount)
  {
    scrobbles.AddOrUpdate(
      song.Id,
      scrobbleCount,
      (_, oldCount) => oldCount + scrobbleCount);
    return scrobbles;
  }
}