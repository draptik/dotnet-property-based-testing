module TestsXunitCsCheck

open CsCheck
open Swensen.Unquote
open Xunit

[<Fact(Skip = "Fails on purpose")>]
let foo () =
  Gen.String.Sample (fun s ->
    test <@ s = "a" @>)