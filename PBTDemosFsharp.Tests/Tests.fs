module Tests

open System
open FsToolkit.ErrorHandling
open Xunit
open FsCheck
open FsCheck.Xunit

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

module CustomerStuff =

    type UnverifiedCustomer =
        { FirstName: string
          LastName: string
          Email: string }

    type VerifiedCustomer = { FullName: string; Email: string }

    let verifyCustomer (unverified: UnverifiedCustomer) : Result<VerifiedCustomer, string> =
        result {
            let! email =
                if unverified.Email |> String.IsNullOrEmpty then
                    Error "Email is required"
                else
                    Ok unverified.Email

            return
                { FullName =
                    match unverified.FirstName, unverified.LastName with
                    | firstName, lastName when
                        not (String.IsNullOrEmpty firstName) && not (String.IsNullOrEmpty lastName)
                        ->
                        $"{firstName} {lastName}"
                    | firstName, _ when not (String.IsNullOrEmpty firstName) -> firstName
                    | _, lastName -> lastName
                  Email = email }
        }

    [<Property(Skip = "TODO")>]
    let ``Verified customer has at least one name and valid email``
        (firstName: Option<NonEmptyString>, lastName: Option<NonEmptyString>, email: NonEmptyString)
        =
        let unverified =
            { FirstName = firstName |> Option.map _.Get |> Option.defaultValue ""
              LastName = lastName |> Option.map _.Get |> Option.defaultValue ""
              Email = email.Get }

        match verifyCustomer unverified with
        | Ok verified ->
            (not (String.IsNullOrEmpty unverified.FirstName)
             || not (String.IsNullOrEmpty unverified.LastName))
            && verified.Email = unverified.Email
            && verified.FullName = match unverified.FirstName, unverified.LastName with
                                   | firstName, lastName when
                                       not (String.IsNullOrEmpty firstName) && not (String.IsNullOrEmpty lastName)
                                       ->
                                       $"{firstName} {lastName}"
                                   | firstName, _ when not (String.IsNullOrEmpty firstName) -> firstName
                                   | _, lastName -> lastName
        | Error _ ->
            String.IsNullOrEmpty unverified.FirstName
            && String.IsNullOrEmpty unverified.LastName

module GildedRose =

    type Item =
        { Name: string
          SellIn: int
          Quality: int }

    let updateQuality (item: Item) : Item =
        match item.Name with
        | "Aged Brie" ->
            let newQuality = min 50 (item.Quality + 1)

            { item with
                SellIn = item.SellIn - 1
                Quality = newQuality }
        | "Sulfuras, Hand of Ragnaros" -> item // Legendary item, does not change
        | "Backstage passes to a TAFKAL80ETC concert" ->
            let newQuality =
                if item.SellIn > 10 then item.Quality + 1
                elif item.SellIn > 5 then item.Quality + 2
                elif item.SellIn > 0 then item.Quality + 3
                else 0 // After the concert, quality drops to 0

            { item with
                SellIn = item.SellIn - 1
                Quality = min 50 newQuality }
        | _ ->
            let newQuality =
                if item.SellIn > 0 then
                    item.Quality - 1
                else
                    item.Quality - 2

            { item with
                SellIn = item.SellIn - 1
                Quality = max 0 newQuality }

    [<Property>]
    let ``Quality remains between 0 and 50 after update`` (name: string, sellIn: int, quality: int) =
        let item =
            { Name = name
              SellIn = sellIn
              Quality = quality }

        let updatedItem = updateQuality item
        updatedItem.Quality >= 0 && updatedItem.Quality <= 50

    [<Property>]
    let ``SellIn decreases by 1 unless it is Sulfuras`` (name: string, sellIn: int, quality: int) =
        let item =
            { Name = name
              SellIn = sellIn
              Quality = quality }

        let updatedItem = updateQuality item

        if name = "Sulfuras, Hand of Ragnaros" then
            updatedItem.SellIn = sellIn
        else
            updatedItem.SellIn = sellIn - 1

    [<Property>]
    let ``Aged Brie quality increases, but not beyond 50`` (sellIn: int, quality: int) =
        let item =
            { Name = "Aged Brie"
              SellIn = sellIn
              Quality = quality }

        let updatedItem = updateQuality item

        if quality < 50 then
            updatedItem.Quality = quality + 1
        else
            updatedItem.Quality = 50

    [<Property>]
    let ``Backstage passes quality drops to 0 after the concert`` (sellIn: int, quality: int) =
        let item =
            { Name = "Backstage passes to a TAFKAL80ETC concert"
              SellIn = sellIn
              Quality = quality }

        let updatedItem = updateQuality item
        if sellIn <= 0 then updatedItem.Quality = 0 else true
