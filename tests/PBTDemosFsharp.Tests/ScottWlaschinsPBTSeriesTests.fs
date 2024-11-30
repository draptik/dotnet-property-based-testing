module ScottWlaschinsPBTSeriesTests

open Xunit

let rand = System.Random()
let randInt () = rand.Next()

// EDFH: Enterprise Developer From Hell
module Part1 =

    open ScottWlaschinsPBTSeries.Part1

    [<Fact>]
    let ``When I add 1 + 2, I expect 3`` () =
        let result = addV1 1 2
        Assert.Equal(3, result)

    [<Fact>]
    let ``When I add 2 + 2, I expect 4`` () =
        let result = addV2 2 2
        Assert.Equal(4, result)

    [<Fact>]
    let ``Add two numbers, expect their sum`` () =
        let testData = [ (1, 2, 3); (2, 2, 4); (3, 5, 8); (27, 15, 42) ]

        for x, y, expected in testData do
            let actual = addV3 x y
            Assert.Equal(expected, actual)

    [<Fact>]
    let ``Add two random numbers, expect their sum`` () =
        for _ in [ 1..100 ] do
            let x = randInt ()
            let y = randInt ()
            let expected = x + y // <- Bad idea! This just reimplements the add function.
            let actual = addV4 x y
            Assert.Equal(expected, actual)

    // First property: What is a difference between `add` and `subtract`?
    [<Fact>]
    let ``add does not depend on parameter order`` () =
        for _ in [ 1..100 ] do
            let x = randInt ()
            let y = randInt ()
            let result1 = addV5 x y
            let result2 = addV5 y x
            Assert.Equal(result1, result2)

    // Second property (we could compare `add` with `multiply`, but we don't want to rely on the presence of `multiply`)
    // What happens when we call the `add` method multiple times?
    [<Fact>]
    let ``add 1 twice is same as add two`` () =
        for _ in [ 1..100 ] do
            let x = randInt ()
            let result1 = x |> addV6 1 |> addV6 1
            let result2 = x |> addV6 2
            Assert.Equal(result1, result2)

    // Third property: What happens when we add zero?
    [<Fact>]
    let ``add zero is same as doing nothing`` () =
        for _ in [ 1..100 ] do
            let x = randInt ()
            let result1 = x |> addV7 0
            let result2 = x
            Assert.Equal(result1, result2)

module Part1Refactored =

    open ScottWlaschinsPBTSeries.Part1Refactored

    let propertyCheck property =
        // property has type: int -> int -> bool
        for _ in [ 1..100 ] do
            let x = randInt ()
            let y = randInt ()
            let result = property x y
            Assert.True(result)

    let commutativeProperty x y =
        let result1 = add x y
        let result2 = add y x
        result1 = result2

    [<Fact>]
    let ``add does not depend on parameter order`` () = propertyCheck commutativeProperty

    let add1TwiceIsAdd2Property x _ =
        let result1 = x |> add 1 |> add 1
        let result2 = x |> add 2
        result1 = result2

    [<Fact>]
    let ``add 1 twice is same as add 2`` () = propertyCheck add1TwiceIsAdd2Property

    let identityProperty x _ =
        let result1 = x |> add 0
        result1 = x

    [<Fact>]
    let ``add zero is same as doing nothing`` () = propertyCheck identityProperty
