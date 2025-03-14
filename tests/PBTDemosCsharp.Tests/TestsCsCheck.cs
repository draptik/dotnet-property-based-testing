using CsCheck;

using Shouldly;

using Xunit.Abstractions;

namespace PBTDemosCsharp.Tests;

public class TestsCsCheck(ITestOutputHelper testOutputHelper)
{
  [Fact]
  public void Generated_numbers_are_in_scope()
  {
    var gen = Gen.Int.Positive.Where(x => x < 100);
    gen.Sample(s =>
      {
        testOutputHelper.WriteLine($"Generated {s}");
        return s <= 100;
      }
    );
  }

  // Example from https://www.gerbenvanadrichem.com/quality-assurance/generating-custom-random-inputs-for-your-property-based-test-in-c-net-with-cscheck/
  [Fact]
  public void EachInputMustBeCorrect()
  {
    // NOTE: The `Sample` function is invoked after calling the `List` property on the `Generate` property.
    MyGenerator.Generate.List.Sample(
      inputs =>
      {
        if (inputs.Count == 0)
        {
          return;
        }

        // testOutputHelper.WriteLine($"Generated {inputs.Count}");
        inputs.ShouldAllBe(x => MustBeCorrect(x));
      });
  }

  // NOTE: This is the "Property" being tested
  private static bool MustBeCorrect(MyClass obj)
  {
    var b1 = obj.MyInt > 0;
    var s = obj.MyString;
    var b2 = s == "\u274c" || s == "\u2713" || s.Length is >= 10 and <= 20;
    return b1 && b2;
  }

  private class MyClass
  {
    public int MyInt { get; init; }
    public required string MyString { get; init; }

    public override string ToString()
    {
      return $"MyInt: {MyInt}, MyString: {MyString}";
    }
  }

  private static class MyGenerator
  {
    public static Gen<MyClass> Generate =>
      from i in Gen.Int
      where i > 0
      from s in Gen.OneOf(
        Gen.String[10, 20],
        Gen.Const("\u274c"), // x
        Gen.Const("\u2713") // ✓
      )
      select new MyClass { MyInt = i, MyString = s };
  }
}