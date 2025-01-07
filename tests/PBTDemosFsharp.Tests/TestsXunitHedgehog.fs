module TestsXunitHedgehog

open System
open Xunit
open Hedgehog
open Hedgehog.Xunit

[<Property>]
let ``hello world`` (xs: int list) =
  // xs |> List.rev = xs // <- fails
  xs |> List.rev |> List.rev = xs

[<Fact(Skip = "Failing Demo w/ Shrinker")>]
let ``Version demo with default shrinking - v1`` () =
  let versionGenerator =
    Range.constantBounded ()
    |> Gen.byte
    |> Gen.map int
    |> Gen.tuple3
    |> Gen.map (fun (ma, mi, bu) -> Version(ma, mi, bu))

  property {
    let! xs = Gen.list (Range.linear 0 100) versionGenerator
    return xs |> List.rev = xs // <- this fails on purpose!
  }
  |> Property.checkBool


let versionGenerator2 =
  Range.constantBounded ()
  |> Gen.byte
  |> Gen.map int
  |> Gen.tuple3
  |> Gen.map (fun (ma, mi, bu) -> Version(ma, mi, bu))

type MyGenContainer =
  static member __ = GenX.defaults |> AutoGenConfig.addGenerator versionGenerator2

[<Property(typeof<MyGenContainer>, Skip = "Failing Demo w/ Shrinker")>]
let ``Version demo with default shrinking - v2`` (versions: Version list) =
  // versions |> List.rev |> List.rev = versions
  versions |> List.rev = versions // <- this fails on purpose!