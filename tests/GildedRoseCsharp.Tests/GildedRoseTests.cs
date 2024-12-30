using FsCheck;
using FsCheck.Xunit;

using GildedRose;

namespace GildedRoseCsharp.Tests;

public class GildedRoseTests
{
  [Property]
  public bool Quality_remains_between_0_and_50_after_update(string name, int sellIn, PositiveInt quality)
  {
    var item = new Item { Name = name, Quality = quality.Get, SellIn = sellIn };
    var actual = item.UpdateQuality();
    return actual.Quality is >= 0 and <= 50;
  }

  [Property]
  public bool SellIn_decreases_by_1_unless_it_is_Sulfuras(string name, int sellIn, PositiveInt quality)
  {
    var item = new Item { Name = name, Quality = quality.Get, SellIn = sellIn };
    var actual = item.UpdateQuality();
    return name == "Sulfuras, Hand of Ragnaros"
      ? actual.SellIn == sellIn
      : actual.SellIn == sellIn - 1;
  }

  [Property]
  public bool Aged_Brie_quality_increases_but_not_beyond_50(PositiveInt sellIn, PositiveInt quality)
  {
    var item = new Item { Name = "Aged Brie", Quality = quality.Get, SellIn = sellIn.Get };
    var actual = item.UpdateQuality();
    return quality.Get < 50
      ? actual.Quality == quality.Get + 1
      : actual.Quality == 50;
  }

  [Property]
  public bool Backstage_passes_quality_drops_to_0_after_the_concert(int sellIn, PositiveInt quality)
  {
    var item = new Item { Name = "Backstage passes to a TAFKAL80ETC concert", Quality = quality.Get, SellIn = sellIn };
    var actual = item.UpdateQuality();
#pragma warning disable IDE0075

    // ReSharper disable once SimplifyConditionalTernaryExpression
    return sellIn <= 0
      ? actual.Quality == 0
      : true;

#pragma warning restore IDE0075
  }

  [Property(Arbitrary = [typeof(ItemArb)], Verbose = true)]
  public bool SellIn_decreases_by_1_unless_it_is_Sulfuras_v2(Item item)
  {
    var sellIn = item.SellIn; // store original value (item is mutable)
    var actual = item.UpdateQuality();
    return item.Name == "Sulfuras, Hand of Ragnaros"
      ? actual.SellIn == sellIn
      : actual.SellIn == sellIn - 1;
  }

  // NOTE: class must be static
  private static class ItemArb
  {
    // NOTE: method must be public static
    // ReSharper disable once UnusedMember.Local
    public static Arbitrary<Item> Generate()
    {
      var numberGen = Gen.Choose(1, 100);
      var itemNameGen = Gen.Elements(
        "Aged Brie",
        "Sulfuras, Hand of Ragnaros",
        "Backstage passes to a TAFKAL80ETC concert");

      var itemGen =
        from name in itemNameGen
        from sellIn in numberGen
        from quality in numberGen
        select new Item { Name = name, SellIn = sellIn, Quality = quality };

      return itemGen.ToArbitrary();
    }
  }
}