using FsCheck.Xunit;

namespace PBTDemosCsharp.Tests;

public class TestsXunit
{
  [Property]
  public bool Reversing_a_list_twice_gives_the_original_list(List<int> list)
  {
    var actual = list.AsEnumerable().Reverse().Reverse();
    var expected = list;
    return actual.SequenceEqual(expected);
  }
}