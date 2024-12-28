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

  // TODO: Create a generator with names from the domain
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
}