#r "nuget:Hedgehog"

open Hedgehog
open System

let version =
  Range.constantBounded ()
  |> Gen.byte
  |> Gen.map int
  |> Gen.tuple3
  |> Gen.map (fun (ma, mi, bu) -> Version(ma, mi, bu))

property {
  let! xs = Gen.list (Range.linear 0 100) version
  return xs |> List.rev = xs
}
|> Property.renderBool
|> printfn "%s"