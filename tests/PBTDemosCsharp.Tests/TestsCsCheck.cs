using CsCheck;

using FluentAssertions;

using Xunit.Abstractions;

namespace PBTDemosCsharp.Tests;

public class TestsCsCheck
{
  private readonly ITestOutputHelper output;

  public TestsCsCheck(ITestOutputHelper testOutputHelper)
  {
    output = testOutputHelper;
  }

  [Fact]
  public void Generated_numbers_are_in_scope()
  {
    var gen = Gen.Int.Positive.Where(x => x < 100);
    gen.Sample(s =>
      {
        output.WriteLine($"Generated {s}");
        return s <= 100;
      }
    );
  }

  // Example from https://www.gerbenvanadrichem.com/quality-assurance/generating-custom-random-inputs-for-your-property-based-test-in-c-net-with-cscheck/
  [Fact]
  public void EachInputMustBeCorrect()
  {
    // NOTE: The `Sample` function is invoked after calling the `List` property on the `Generate` property.
    MyGenerator.Generate.List.Sample(inputs =>
    {
      if (inputs.Count == 0)
      {
        return;
      }

      output.WriteLine($"Generated {inputs.Count}");
      _ = inputs.Should().AllSatisfy(MustBeCorrect);
    });
  }

  // NOTE: This is the "Property" being tested
  private void MustBeCorrect(MyClass obj)
  {
    _ = obj.MyInt.Should().BeGreaterThan(0);
    _ = obj.MyString.Should().Match(s => s == "\u274c" || s == "\u2713" || (s.Length >= 10 && s.Length <= 20));
  }

  private class MyClass
  {
    public int MyInt { get; init; }
    public required string MyString { get; init; }
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