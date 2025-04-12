module SongServiceTests

open SongRecommendations
open System
open System.Threading.Tasks
open Xunit
open FsCheck.FSharp
open FsCheck.Xunit

// "Icebreaker Test"
[<Fact>]
let ``No Data (icebreaker test)`` () =
  task {
    let srv = FakeSongService ()
    let sut = RecommendationsProvider srv
    let! actual = sut.GetRecommendationsAsync "user name without data"
    Assert.Empty actual
  }

module Gen =
  let alphaNumeric = Gen.elements (['a'..'z'] @ ['A'..'Z'] @ ['0'..'9'])

  // create an alphanumeric name
  // - with length between 1 and 10
  // - and always starting with a letter
  let userName =
    gen {
      let! length = Gen.choose (1, 10)
      let! firstLetter = Gen.elements <| ['a'..'z'] @ ['A'..'Z']
      let! rest = alphaNumeric |> Gen.listOfLength length
      return firstLetter :: rest |> List.toArray |> String
    }

  // creates a random Song
  let song =
    ArbMap.generate ArbMap.defaults |> Gen.map Song

[<Property>]
let ``No Data (property-based test)`` () =
  Gen.userName
  |> Arb.fromGen
  |> Prop.forAll
  <| fun userName ->
    task {
      let srv = FakeSongService ()
      let sut = RecommendationsProvider srv
      let! actual = sut.GetRecommendationsAsync userName
      Assert.Empty actual
    }
    :> Task

[<Property>]
let ``One User, some songs`` () =
  gen {
    let! user = Gen.userName
    let! songs = Gen.arrayOf Gen.song
    let! scrobbleCounts =
      Gen.choose (1, 100)
      |> Gen.arrayOfLength songs.Length
    return (user, Array.zip songs scrobbleCounts)
  }
  |> Arb.fromGen
  |> Prop.forAll
  <| fun (user, scrobbles) ->
    task {
      let srv = FakeSongService ()
      scrobbles |> Array.iter (fun (s, c) -> srv.Scrobble (user, s, c))
      let sut = RecommendationsProvider srv
      let! actual = sut.GetRecommendationsAsync user
      Assert.Empty actual
    }
    :> Task