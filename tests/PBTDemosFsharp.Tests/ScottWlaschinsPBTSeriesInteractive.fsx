#r "nuget:FsCheck"
open FsCheck

let add x y = x * y

let add1TwiceIsAdd2Property x =
    let result1 = x |> add 1 |> add 1
    let result2 = x |> add 2
    result1 = result2

Check.Quick add1TwiceIsAdd2Property
