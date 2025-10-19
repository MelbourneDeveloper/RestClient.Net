using Xunit;

#pragma warning disable CS8509 // C# compiler doesn't understand nested discriminated unions - use EXHAUSTION001 instead
#pragma warning disable SA1512 // Single-line comments should not be followed by blank line

namespace NucliaDbClient.Tests;

[CollectionDefinition("NucliaDB Tests")]
public sealed class NucliaDbTestList : ICollectionFixture<NucliaDbFixture> { }
