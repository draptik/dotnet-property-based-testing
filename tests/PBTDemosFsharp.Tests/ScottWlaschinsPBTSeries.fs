module ScottWlaschinsPBTSeries

// EDFH: Enterprise Developer From Hell
module Part1 =

    // V1
    let addV1 x y = if x = 1 && y = 2 then 3 else 0

    // V2 applying the Transformation Priority Premise
    let addV2 x y =
        if x = 1 && y = 2 then 3
        else if x = 2 && y = 2 then 4
        else 0

    // V3 no problem...
    let addV3 x y =
        match x, y with
        | 1, 2 -> 3
        | 2, 2 -> 4
        | 3, 5 -> 8
        | 27, 15 -> 42
        | _ -> 0

    // V4 addV4
    let addV4 x y = x + y

    // V5 ;-)
    let addV5 x y = x * y

    // V6
    // let addV5 x y = 0
    let addV6 x y = 0

    // V7
    let addV7 x y = x + y

module Part1Refactored =

    let add x y = x + y
