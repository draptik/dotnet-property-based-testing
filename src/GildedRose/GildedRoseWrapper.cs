using System.Text;

namespace GildedRose;

public static class GildedRoseWrapper
{
  // This is a wrapper around the UpdateQuality method.
  // It's only purpose is to return a testable string.
  public static string Run(int days, List<Item> items)
  {
    var app = new GildedRose(items);

    var sb = new StringBuilder();

    for (var day = 0; day < days; day++)
    {
      sb = sb.AppendLine($"-------- day {day}  --------");
      sb = sb.AppendLine("name, sellIn, quality");
      sb = items.Aggregate(
        sb, (current, item) =>
          current.AppendLine(item.Name + " " + item.SellIn + " - " + item.Quality));
      sb = sb.AppendLine("");

      app.UpdateQuality();
    }

    return sb.ToString();
  }

  // Helper method for updating a single item.
  public static Item UpdateQuality(this Item item)
  {
    var items = new List<Item> { item };
    var app = new GildedRose(items);
    app.UpdateQuality();
    return items.First();
  }
}