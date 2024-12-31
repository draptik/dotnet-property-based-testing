module TestsXunitCsCheck

open CsCheck
open Swensen.Unquote
open Xunit

[<Fact>]
let ``generated numbers are in scope`` () =
  let gen = Gen.Int.Positive.Where(fun x -> x < 100)
  gen.Sample(fun s -> test <@ s < 100 @>)