module Tests_nunit

open NUnit.Framework
open FsCheck

module HelloWorld =

  [<Test>]
  let ``reversing a list twice gives original list`` () =
    let checkFn (aList: int list) = List.rev (List.rev aList) = aList

    Check.Verbose checkFn