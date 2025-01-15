# Property-based testing demos with dotnet

Investigating dotnet property-based testing (PBT) libraries:

- [FsCheck](https://fscheck.github.io/FsCheck/)
- [Hedgehog](https://hedgehogqa.github.io/fsharp-hedgehog/)
- [CsCheck](https://github.com/AnthonyLloyd/CsCheck)

Goal: Comparing C# and F# usage and providing simple usage examples

- up-to-date: does it work with current dotnet versions (dotnet8, dotnet9)?
- integration with common test frameworks (xUnit, NUnit, ...)
- documentation
- external API design: do we have to dive into the source code to understand usage?
- differences:
  - generators / arbitraries
  - performance
  - shrinker