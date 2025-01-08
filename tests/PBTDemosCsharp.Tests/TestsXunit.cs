using FsCheck;
using FsCheck.Fluent;
using FsCheck.Xunit;

namespace PBTDemosCsharp.Tests;

public class TestsXunit
{
  // FsCheck API, not using FsCheck.Xunit
  // Uses Prop.ForAll and QuickCheckThrowOnFailure
  [Fact]
  public void Reversing_a_list_twice_gives_the_original_list_v1()
  {
    static bool CheckFn(List<int> list)
    {
      return list.AsEnumerable().Reverse().Reverse().SequenceEqual(list);
    }

    // The lambda creates the test data input
    Prop.ForAll((List<int> list) => CheckFn(list)).QuickCheckThrowOnFailure();
  }

  // Using FsCheck.Xunit's Property attribute
  [Property]
  public bool Reversing_a_list_twice_gives_the_original_list_v2(List<int> list)
  {
    var actual = list.AsEnumerable().Reverse().Reverse();
    var expected = list;
    return actual.SequenceEqual(expected);
  }
}