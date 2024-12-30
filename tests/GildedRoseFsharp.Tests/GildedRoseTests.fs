module GildedRoseTests

open FsCheck
open FsCheck.Xunit
open GildedRose

let generateItem (name: string) (sellIn: int) (quality: PositiveInt) =
  let item = Item()
  item.Name <- name
  item.SellIn <- sellIn
  item.Quality <- quality.Get
  item

[<Property>]
let ``quality remains between 0 and 50 after update`` (name: string, sellIn: int, quality: PositiveInt) =
  let item = generateItem name sellIn quality
  let actual = item.UpdateQuality()
  actual.Quality >= 0 && actual.Quality <= 50

[<Property>]
let ``sellIn decreases by 1 unless it is Sulfuras`` (name: string, sellIn: int, quality: PositiveInt) =
  let item = generateItem name sellIn quality
  let actual = item.UpdateQuality()

  if name = "Sulfuras, Hand of Ragnaros" then
    actual.SellIn = sellIn
  else
    actual.SellIn = sellIn - 1

[<Property>]
let ``aged brie quality increases but not beyond 50`` (sellIn: PositiveInt, quality: PositiveInt) =
  let item = generateItem "Aged Brie" sellIn.Get quality
  let actual = item.UpdateQuality()

  if quality.Get < 50 then
    actual.Quality = quality.Get + 1
  else
    actual.Quality = 50

type ItemArb =
  static member Generate() =
    gen {
      let! name =
        Gen.elements [
          "Aged Brie"
          "Sulfuras, Hand of Ragnaros"
          "Backstage passes to a TAFKAL80ETC concert"
        ]

      let! sellIn = Gen.choose (1, 100)
      let! quality = Arb.generate<PositiveInt>
      return generateItem name sellIn quality
    }
    |> Arb.fromGen

[<Property(Arbitrary = [| typeof<ItemArb> |], Verbose = true)>]
let ``sellIn decreases by 1 unless it is Sulfuras (v2)`` (item: Item) =
  let sellIn = item.SellIn // store original value (item is mutable)
  let actual = item.UpdateQuality()

  if item.Name = "Sulfuras, Hand of Ragnaros" then
    actual.SellIn = sellIn
  else
    actual.SellIn = sellIn - 1