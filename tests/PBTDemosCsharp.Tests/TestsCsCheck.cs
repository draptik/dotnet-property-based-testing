using CsCheck;

namespace PBTDemosCsharp.Tests;

public class TestsCsCheck
{
  [Fact(Skip = "Fails on purpose")]
  public void Failing_test()
  {
    Gen.String.Sample(s => Assert.Equal("a", s));
  }
}