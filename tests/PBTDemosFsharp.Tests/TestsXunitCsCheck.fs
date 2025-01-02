module TestsXunitCsCheck

open System
open CsCheck
open Swensen.Unquote
open Xunit

[<Fact>]
let ``generated numbers are in scope`` () =
  let gen = Gen.Int.Positive.Where(fun x -> x < 100)
  gen.Sample(fun s -> test <@ s < 100 @>)

type MyClass = {
  MyInt: int
  MyString: string
}

[<Fact>]
let ``each input must be correct`` () =

  // let toCSharpFunc (f: Gen<'a> -> bool) : Func<Gen<'a>, bool> =
  //   Func<Gen<'a>, bool>(f)

  let myClassGenerator : Gen<MyClass> =
      Gen.Select(
        Gen.Int,
        Gen.String[10, 20],
        fun (i, s) -> { MyInt = i; MyString = s })

  let gs = myClassGenerator.List

  let f a : unit = ()
  Check.Sample(gs, f)
