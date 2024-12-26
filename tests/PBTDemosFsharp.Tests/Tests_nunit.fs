module Tests_nunit

open NUnit.Framework
open FsUnit
open FsCheck

module HelloWorld =

  [<Test>]
  let ``reversing a list twice gives original list`` () =
    let checkFn (aList: int list) =
      List.rev (List.rev aList) |> should equal aList

    Check.Verbose checkFn