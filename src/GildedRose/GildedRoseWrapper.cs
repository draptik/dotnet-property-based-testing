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

    for (var i = 0; i < days; i++)
    {
      sb = sb.AppendLine($"-------- day {i}  --------");
      sb = sb.AppendLine("name, sellIn, quality");
      sb = items.Aggregate(
        sb, (current, t) =>
          current.AppendLine(t.Name + " " + t.SellIn + " - " + t.Quality));
      sb = sb.AppendLine("");

      app.UpdateQuality();
    }

    return sb.ToString();
  }
}