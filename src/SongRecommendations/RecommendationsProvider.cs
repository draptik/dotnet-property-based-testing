using System.Diagnostics.CodeAnalysis;

namespace SongRecommendations;

[SuppressMessage("Style", "IDE0022:Use block body for method")]
public class RecommendationsProvider(ISongService songService)
{
  // NOTE: Usage is not detected because the method is called from an F# test
  [SuppressMessage("ReSharper", "UnusedMember.Global")]
  public async Task<IReadOnlyCollection<Song>> GetRecommendationsAsync(string userName)
  {
    // Impure
    var scrobbles = await songService.GetTopScrobblesAsync(userName);

    // Pure
    var songIds = HandleOwnScrobbles(scrobbles);

    var recommendationCandidates = new List<Song>();
    foreach (var songId in songIds)
    {
      // Impure
      var otherListeners = await songService
        .GetTopListenersAsync(songId);

      // Pure
      var otherUserNames = HandleOtherListeners(otherListeners);

      foreach (var otherUserName in otherUserNames)
      {
        // Impure
        var otherScrobbles = await songService
          .GetTopScrobblesAsync(otherUserName);

        // Pure
        var songsToRecommend = HandleOtherScrobbles(otherScrobbles);

        recommendationCandidates.AddRange(songsToRecommend);
      }
    }

    // Pure
    return FinalizeRecommendations(recommendationCandidates);
  }

  // Pure
  private static int[] HandleOwnScrobbles(IReadOnlyCollection<Scrobble> scrobbles) =>
    scrobbles
      .OrderByDescending(s => s.ScrobbleCount)
      .Take(100)
      .Select(s => s.Song.Id)
      .ToArray();

  // Pure
  private static string[] HandleOtherListeners(IReadOnlyCollection<User> users) =>
    users
      .Where(u => u.TotalScrobbleCount >= 10_000)
      .OrderByDescending(u => u.TotalScrobbleCount)
      .Take(20)
      .Select(u => u.UserName)
      .ToArray();

  // Pure
  private static Song[] HandleOtherScrobbles(IReadOnlyCollection<Scrobble> scrobbles) =>
    scrobbles
      .Where(s => s.Song.IsVerifiedArtist)
      .OrderByDescending(s => s.Song.Rating)
      .Take(10)
      .Select(s => s.Song)
      .ToArray();

  // Pure
  private static Song[] FinalizeRecommendations(IReadOnlyCollection<Song> songs) =>
    songs
      .OrderByDescending(s => s.Rating)
      .Take(200)
      .ToArray();
}