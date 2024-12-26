using FsCheck;
using FsCheck.NUnit;

using NUnit.Framework;

using Property = FsCheck.NUnit.PropertyAttribute;

namespace PBTDemosCsharp.Tests;

public class TestsNunit
{
  // FsCheck API, not using FsCheck.NUnit
  // Uses Prop.ForAll and QuickCheckThrowOnFailure
  [Test]
  public void Reversing_a_list_twice_gives_the_original_list_v1()
  {
    static bool CheckFn(List<int> list)
    {
      return list.AsEnumerable().Reverse().Reverse().SequenceEqual(list);
    }

    // The lambda creates the test data input
    Prop.ForAll((List<int> list) => CheckFn(list)).QuickCheckThrowOnFailure();
  }

  // Using FsCheck.NUnit's Property attribute
  [Property]
  public bool Reversing_a_list_twice_gives_the_original_list_v2(List<int> list)
  {
    var actual = list.AsEnumerable().Reverse().Reverse();
    var expected = list;
    return actual.SequenceEqual(expected);
  }
}