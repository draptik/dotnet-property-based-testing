using CsCheck;

using FluentAssertions;

namespace PBTDemosCsharp.Tests;

public class TestsCsCheck
{
  [Fact]
  public void Generated_numbers_are_in_scope()
  {
    var gen = Gen.Int.Positive.Where(x => x < 100);
    gen.Sample(s =>
      s.Should().BeLessThan(100));
  }
}