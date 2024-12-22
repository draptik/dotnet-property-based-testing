#r "nuget:FsCheck"
open FsCheck

// Keyboard shortcuts:
// Rider (custom keybinding): Ctrl+Alt+i
// Rider (custom IdeaVim binding): <leader>Fi

// Part2 - Introducing FsCheck ================================================

let addV0 x y = x * y

let add1TwiceIsAdd2Property x =
  let result1 = x |> addV0 1 |> addV0 1
  let result2 = x |> addV0 2
  result1 = result2

Check.Quick add1TwiceIsAdd2Property

// Intruding Generators -------------------------------------------------------

let intGenerator = Arb.generate<int>

// Gen.sample:
// first arg -> number of elements
// second arg -> "size" (for int: max value)
// third arg -> generator
Gen.sample 1 3 intGenerator
Gen.sample 10 3 intGenerator
Gen.sample 100 3 intGenerator

// see how the values are clustered around the center point
intGenerator
|> Gen.sample 10 1000
|> Seq.groupBy id // use the generated number as key
|> Seq.map (fun (k, v) -> (k, Seq.length v)) // count the occurences
|> Seq.sortBy fst // sort by key
|> Seq.toList

let tupleGenerator = Arb.generate<int * int * int>

// generate 3 tuples with a maximum size of 1
Gen.sample 1 3 tupleGenerator

// generate 3 tuples with a maximum size of 10
Gen.sample 10 3 tupleGenerator

// generate 3 tuples with a maximum size of 100
Gen.sample 100 3 tupleGenerator

let intOptionGenerator = Arb.generate<int option>
// generate 10 int options with a maximum size of 5
Gen.sample 5 10 intOptionGenerator

let intListGenerator = Arb.generate<int list>
// generate 10 int lists with a maximum size of 5
Gen.sample 5 10 intListGenerator

let stringGenerator = Arb.generate<string>
// generate 3 strings with a maximum size of 1
Gen.sample 1 3 stringGenerator
// generate 3 strings with a maximum size of 10
Gen.sample 10 3 stringGenerator

type Color =
  | Red
  | Green of int
  | Blue of bool

let colorGenerator = Arb.generate<Color>
Gen.sample 50 10 colorGenerator


type Point = { x: int; y: int; color: Color }
let pointGenerator = Arb.generate<Point>
Gen.sample 50 10 pointGenerator

// Shrinking ------------------------------------------------------------------
let isSmallerThan80 x = x < 80

// Fails with number 100:
isSmallerThan80 100
// ...this is what FsCheck does in the background:
Arb.shrink 100 |> Seq.toList // It takes this list and runs the check again...
Arb.shrink 81 |> Seq.toList // It takes this list and runs the check again...
Arb.shrink 80 |> Seq.toList // It takes this list and runs the check again...

Check.Quick isSmallerThan80

let config = { Config.Quick with MaxTest = 1000 }

// `Check.One` is the same as `Check.Quick`, with the advantage that `Check.One` can take a config.
Check.One(config, isSmallerThan80)

// Verbose mode and logging ---------------------------------------------------
// let add x y =
//     if x < 25 || y < 25 then x + y else x * y

// let associativeProperty x y z =
//     let result1 = add x (add y z) // x + (y + y)
//     let result2 = add (add x y) z // (x + y) + z
//     result1 = result2

// Check.Verbose associativeProperty

// For more details, see https://fsharpforfunandprofit.com/posts/property-based-testing-1/

// Adding pre-conditions ------------------------------------------------------
let additionIsNotMultiplication x y = x + y <> x * y

Check.One(config, additionIsNotMultiplication)

let preCondition x y = (x, y) <> (0, 0) && (x, y) <> (2, 2)

let additionIsNotMultiplication_withPreCondition x y =
  preCondition x y ==> additionIsNotMultiplication x y

Check.Quick additionIsNotMultiplication_withPreCondition

// Combining multiple properties
let add x y = x + y

let commutativeProperty x y = add x y = add y x

let associativeProperty x y z = add x (add y z) = add (add x y) z

let leftIdentityProperty x = add x 0 = x

let rightIdentityProperty x = add 0 x = x

type AdditionSpecification =
  static member ``Commutative`` x y = commutativeProperty x y
  static member ``Associative`` x y z = associativeProperty x y z
  static member ``Left Identity`` x = leftIdentityProperty x
  static member ``Right Identity`` x = rightIdentityProperty x

Check.QuickAll<AdditionSpecification>()