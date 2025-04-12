module SongServiceTests

open SongRecommendations
open Xunit

// "Icebreaker Test"
[<Fact>]
let ``No Data`` () =
  task {
    let srvc = FakeSongService ()
    let sut = RecommendationsProvider srvc
    let! actual = sut.GetRecommendationsAsync "user name without data"
    Assert.Empty actual
  }