// ReSharper disable once CheckNamespace
namespace System.Runtime.CompilerServices;

/// <summary>
/// Reserved for use by the compiler for tracking metadata.
/// This class should not be used by developers in source code.
/// </summary>
[Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
#pragma warning disable CA1812 // Internal class is never instantiated - This is used by the compiler for init-only properties
#pragma warning disable CA1852 // Type can be sealed - This must remain unsealed for compiler use
internal class IsExternalInit { }
#pragma warning restore CA1852
#pragma warning restore CA1812
