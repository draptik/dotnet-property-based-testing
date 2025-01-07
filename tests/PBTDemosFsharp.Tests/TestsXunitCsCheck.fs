module TestsXunitCsCheck

open System
open CsCheck
open Swensen.Unquote
open Xunit
open Xunit.Abstractions

[<Fact>]
let ``generated numbers are in scope`` () =
  let gen = Gen.Int.Positive.Where(fun x -> x < 100)
  gen.Sample(fun s -> test <@ s < 100 @>)

type MyClass = { MyInt: int; MyString: string }

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

  let myClassGenerator: Gen<MyClass> =
    Gen.Select(Gen.Int.Positive, Gen.String[10, 20], Func<int, string, MyClass>(fun i s -> { MyInt = i; MyString = s }))

  myClassGenerator.List.Sample(fun x ->
    if x.Count = 0 then
      true = true
    else
      let actual =
        x.TrueForAll(fun el -> el.MyInt > 0 && el.MyString.Length >= 10 && el.MyString.Length <= 20)

      actual = true)

type VersionShrinkingDemo(output: ITestOutputHelper) =

  // Adopted from https://github.com/AnthonyLloyd/CsCheck/blob/330d8a497949b4dba18d94efe24d94441de0e53a/Comparison.md?plain=1#L40
  [<Fact(Skip = "Failing Demo w/ Shrinker")>]
  let ``random version number shrinking demo`` () =
    let versionListGenerator: Gen<Version> =
      Gen.Byte
        .Select(int)
        .Select(fun x -> (x, x, x))
        .Select(fun (ma, mi, bu) -> Version(ma, mi, bu))

    let reverse (xs: Collections.Generic.List<'a>) = xs |> Seq.toList |> List.rev

    versionListGenerator.List.Sample(fun xs ->
      let input = xs |> Seq.toList
      let actual = reverse xs
      output.WriteLine($"input: %A{input}")
      output.WriteLine($"actual: %A{actual}")
      Check.Equal(actual, input))