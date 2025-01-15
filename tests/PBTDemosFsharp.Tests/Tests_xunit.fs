module Tests_xunit

open System
open FsToolkit.ErrorHandling
open Xunit
open FsCheck
open FsCheck.Xunit

module HelloWorld =

  let reverseList (list: int list) : int list = list |> List.rev

  [<Fact>]
  let ``reversing a list twice gives original list`` () =
    let checkFn (aList: int list) = List.rev (List.rev aList) = aList

    Check.QuickThrowOnFailure checkFn

  // Using `Check.Verbose` only works on the command line, not in Rider.
  // Check.Verbose checkFn

  [<Property>]
  let ``reversing a list twice gives original list - version 2`` (aList: int list) =
    let actual = aList |> List.rev |> List.rev
    let expected = aList
    actual = expected

  [<Property(Verbose = true)>]
  let ``reversing a list twice gives original list - version 2 (verbose)`` (aList: int list) =
    let actual = aList |> List.rev |> List.rev
    let expected = aList
    actual = expected

// FizzBuzz is actually an "advanced" example in the PBT context, because it requires knowledge of
// Arbitraries and Generators
module FizzBuzzing =

  let fizzbuzz n =
    match n % 3, n % 5 with
    | 0, 0 -> "FizzBuzz"
    | 0, _ -> "Fizz"
    | _, 0 -> "Buzz"
    | _ -> n.ToString()

  module FactArbPropCheckPattern =

    [<Fact>]
    let ``Is a number`` () =
      let arb =
        Gen.choose (0, 100)
        |> Gen.filter (fun x -> x % 3 <> 0 && x % 5 <> 0)
        |> Arb.fromGen

      let property number = (fizzbuzz number) = number.ToString()
      Prop.forAll arb property |> Check.QuickThrowOnFailure

    [<Fact>]
    let ``Divisible by 3`` () =
      let arb = Gen.choose (0, 100) |> Gen.map (fun x -> x * 3) |> Arb.fromGen
      let property number = (fizzbuzz number).StartsWith("Fizz")
      Prop.forAll arb property |> Check.QuickThrowOnFailure

    [<Fact>]
    let ``Divisible by 5`` () =
      let arb =
        Gen.choose (0, 100)
        |> Gen.map (fun x -> x * 5)
        |> Gen.filter (fun x -> x % 3 <> 0)
        |> Arb.fromGen

      let property number = (fizzbuzz number).StartsWith("Buzz")
      Prop.forAll arb property |> Check.QuickThrowOnFailure

    [<Fact>]
    let ``Divisible by 15`` () =
      let arb = Gen.choose (0, 100) |> Gen.map (fun x -> x * 15) |> Arb.fromGen

      let property number =
        (fizzbuzz number).StartsWith("FizzBuzz")

      Prop.forAll arb property |> Check.QuickThrowOnFailure

  module XunitPropertyAttributePattern =

    let nonDivisibleNumberArb =
      Gen.choose (0, 100)
      |> Gen.filter (fun x -> x % 3 <> 0 && x % 5 <> 0)
      |> Arb.fromGen

    type NonDivisibleNumberArb =
      static member NonDivisibleNumber() = nonDivisibleNumberArb

    [<Property(Arbitrary = [| typeof<NonDivisibleNumberArb> |])>]
    let ``Non divisible numbers`` number = (fizzbuzz number) = number.ToString()

    let divisibleByThreeArb =
      Gen.choose (0, 100)
      |> Gen.map (fun x -> x * 3)
      |> Gen.filter (fun x -> x % 5 <> 0)
      |> Arb.fromGen

    type DivisibleByThreeArb =
      static member DivisibleByThree() = divisibleByThreeArb

    [<Property(Arbitrary = [| typeof<DivisibleByThreeArb> |])>]
    let ``Divisible by 3`` number = (fizzbuzz number) = "Fizz"

    let divisibleByFiveArb =
      Gen.choose (0, 100)
      |> Gen.map (fun x -> x * 5)
      |> Gen.filter (fun x -> x % 3 <> 0)
      |> Arb.fromGen

    type DivisibleByFiveArb =
      static member DivisibleByFive() = divisibleByFiveArb

    [<Property(Arbitrary = [| typeof<DivisibleByFiveArb> |])>]
    let ``Divisible by 5`` number = (fizzbuzz number) = "Buzz"

    let divisibleByFifteenArb =
      Gen.choose (0, 100) |> Gen.map (fun x -> x * 15) |> Arb.fromGen

    type DivisibleByFifteenArb =
      static member DivisibleByFifteen() = divisibleByFifteenArb

    [<Property(Arbitrary = [| typeof<DivisibleByFifteenArb> |])>]
    let ``Divisible by 15`` number = (fizzbuzz number) = "FizzBuzz"

// see https://web.archive.org/web/20240910144730/https://blog.ploeh.dk/2021/06/28/property-based-testing-is-not-the-same-as-partition-testing/
// Finding these property is the hard part...
module FizzBuzzingDoneCorrectly =

  let fizzBuzz n =
    match n % 3, n % 5 with
    | 0, 0 -> "FizzBuzz"
    | 0, _ -> "Fizz"
    | _, 0 -> "Buzz"
    | _, _ -> n.ToString()

  [<Property>]
  let ``at least one number in 3 consecutive values`` (i: int) =
    let range = [i..i+2]

    let tryToNumber (s: string) =
      match Int32.TryParse(s) with
      | true, value -> Some value
      | false, _ -> None

    let actual =
      range
      |> List.map fizzBuzz
      |> List.map tryToNumber
      |> List.choose id
      |> List.length

    actual >= 1

  [<Property>]
  let ``only one Buzz in 5 consecutive values`` (i: int) =
    let range = [i..i+4]

    let actual =
      range
      |> List.map fizzBuzz
      |> List.filter (fun s -> s.EndsWith("Buzz"))
      |> List.length

    actual = 1

  [<Property>]
  let ``at least one literal Buzz in 10 values`` (i: int) =
    let range = [i..i+9]

    let actual =
      range
      |> List.map fizzBuzz
      |> List.filter (fun s -> s = "Buzz")
      |> List.length

    actual >= 1

  [<Property>]
  let ``numbers round-trip (there-and-back-again)`` (i: int) =
    let range = [i..i+2]

    let tryToNumber (s: string) =
      match Int32.TryParse(s) with
      | true, value -> Some value
      | false, _ -> None

    let actual =
      range
      |> List.map fizzBuzz
      |> List.map tryToNumber
      |> List.choose id
      |> List.forall (fun x -> List.contains x range)

    actual = true

module CustomerStuff =

  type UnverifiedCustomer = {
    FirstName: string option
    LastName: string option
    Email: string option
  }

  type VerifiedCustomer = { FullName: string; Email: string }

  let verifyCustomer (unverified: UnverifiedCustomer) : Result<VerifiedCustomer, string> =
    result {
      let! email =
        unverified.Email
        |> Option.map Ok
        |> Option.defaultValue (Error "Email is required")

      let! fullName =
        match unverified.FirstName, unverified.LastName with
        | Some firstName, Some lastName -> $"{firstName} {lastName}" |> Ok
        | Some fn, _ -> fn |> Ok
        | _, Some ln -> ln |> Ok
        | None, None -> Error "First and/or Last Name is required"

      return { FullName = fullName; Email = email }
    }

  [<Property>]
  let ``Verified customer has a full name and valid email``
    (firstName: NonEmptyString option, lastName: NonEmptyString option, email: NonEmptyString option)
    =

    let sanitizeNonEmpty (s: string) =
      if String.IsNullOrWhiteSpace(s) then "a" else s

    // helper function: maps `NonEmptyString option` to `string option`
    let toOptionString (input: NonEmptyString option) =
      match input with
      | None -> None
      | Some value -> value.Get |> sanitizeNonEmpty |> Some

    let unverified: UnverifiedCustomer = {
      FirstName = firstName |> toOptionString
      LastName = lastName |> toOptionString
      Email = email |> toOptionString
    }

    match verifyCustomer unverified with
    | Ok verified ->
      verified.Email = unverified.Email.Value // EMail has the correct value
      && not (String.IsNullOrWhiteSpace(verified.FullName)) // FullName has a value
    | Error _ -> true // unverified inputs are recognized as invalid

module GildedRose =

  type Item = {
    Name: string
    SellIn: int
    Quality: int
  }

  let updateQuality (item: Item) : Item =
    match item.Name with
    | "Aged Brie" ->
      let newQuality = min 50 (item.Quality + 1)

      {
        item with
            SellIn = item.SellIn - 1
            Quality = newQuality
      }
    | "Sulfuras, Hand of Ragnaros" -> item // Legendary item, does not change
    | "Backstage passes to a TAFKAL80ETC concert" ->
      let newQuality =
        if item.SellIn > 10 then item.Quality + 1
        elif item.SellIn > 5 then item.Quality + 2
        elif item.SellIn > 0 then item.Quality + 3
        else 0 // After the concert, quality drops to 0

      {
        item with
            SellIn = item.SellIn - 1
            Quality = min 50 newQuality
      }
    | _ ->
      let newQuality =
        if item.SellIn > 0 then
          item.Quality - 1
        else
          item.Quality - 2

      {
        item with
            SellIn = item.SellIn - 1
            Quality = max 0 newQuality
      }

  [<Property>]
  let ``Quality remains between 0 and 50 after update`` (name: string, sellIn: int, quality: int) =
    let item = {
      Name = name
      SellIn = sellIn
      Quality = quality
    }

    let updatedItem = updateQuality item
    updatedItem.Quality >= 0 && updatedItem.Quality <= 50

  [<Property>]
  let ``SellIn decreases by 1 unless it is Sulfuras`` (name: string, sellIn: int, quality: int) =
    let item = {
      Name = name
      SellIn = sellIn
      Quality = quality
    }

    let updatedItem = updateQuality item

    if name = "Sulfuras, Hand of Ragnaros" then
      updatedItem.SellIn = sellIn
    else
      updatedItem.SellIn = sellIn - 1

  [<Property>]
  let ``Aged Brie quality increases, but not beyond 50`` (sellIn: int, quality: int) =
    let item = {
      Name = "Aged Brie"
      SellIn = sellIn
      Quality = quality
    }

    let updatedItem = updateQuality item

    if quality < 50 then
      updatedItem.Quality = quality + 1
    else
      updatedItem.Quality = 50

  [<Property>]
  let ``Backstage passes quality drops to 0 after the concert`` (sellIn: int, quality: int) =
    let item = {
      Name = "Backstage passes to a TAFKAL80ETC concert"
      SellIn = sellIn
      Quality = quality
    }

    let updatedItem = updateQuality item
    if sellIn <= 0 then updatedItem.Quality = 0 else true

module VersionShrinkingDemo =

  [<Fact(Skip = "Failing Demo w/ Shrinker")>]
  let ``random version numbers shrinking demo`` () =
    let versionListArb =
      Arb.generate<byte>
      |> Gen.map int
      |> Gen.three
      |> Gen.map (fun (ma, mi, bu) -> Version(ma, mi, bu))
      |> Gen.listOf
      |> Arb.fromGen

    let property xs = xs |> List.rev = xs
    Prop.forAll versionListArb property |> Check.QuickThrowOnFailure