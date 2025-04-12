module SongServiceTests

open System
open System.Threading.Tasks
open SongRecommendations
open Xunit
open FsCheck.FSharp
open FsCheck.Xunit

// "Icebreaker Test"
[<Fact>]
let ``No Data (icebreaker test)`` () =
  task {
    let srvc = FakeSongService ()
    let sut = RecommendationsProvider srvc
    let! actual = sut.GetRecommendationsAsync "user name without data"
    Assert.Empty actual
  }

module Gen =
  let alphaNumeric = Gen.elements (['a'..'z'] @ ['A'..'Z'] @ ['0'..'9'])

  // create an alphanumeric name
  // - with max length of 10,
  // - and always starting with a letter
  let userName =
    gen {
      let! length = Gen.choose (1, 10)
      let! firstLetter = Gen.elements <| ['a'..'z'] @ ['A'..'Z']
      let! rest = alphaNumeric |> Gen.listOfLength length
      return firstLetter :: rest |> List.toArray |> String
    }

[<Property>]
let ``No Data (property-based test)`` () =
  Gen.userName |> Arb.fromGen |> Prop.forAll <| fun userName ->
    task {
      let srvc = FakeSongService ()
      let sut = RecommendationsProvider srvc
      let! actual = sut.GetRecommendationsAsync userName
      Assert.Empty actual
    } :> Task