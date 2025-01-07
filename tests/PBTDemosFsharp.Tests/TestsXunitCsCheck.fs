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

  // let mapToCSharpFunc f =
  //   Func<int, string, MyClass>(f)
  //
  // let myClassGenerator : Gen<MyClass> =
  //     Gen.Select(
  //       Gen.Int.Positive,
  //       Gen.String[10, 20],
  //       mapToCSharpFunc (fun i s -> { MyInt = i; MyString = s }))

  let myClassGenerator : Gen<MyClass> =
      Gen.Select(
        Gen.Int.Positive,
        Gen.String[10, 20],
        Func<int, string, MyClass>(fun i s -> { MyInt = i; MyString = s }))

  myClassGenerator.List.Sample(fun x ->
    if x.Count = 0 then
      true = true
    else
      let actual =
        x.TrueForAll(fun el ->
          el.MyInt > 0 &&
          el.MyString.Length >= 10 &&
          el.MyString.Length <= 20
        )
      actual = true
  )
