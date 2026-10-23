using Collections.Analyzer.CodeFixes;
using Collections.Analyzer.Diagnostics.CI0008;
using Microsoft.CodeAnalysis.Testing;

namespace Tests.ArrayContainsToHashSetTests;

public class ArrayContainsToFrozenSetVerifier : CodeFixVerifier<
    ArrayContainsToHashSetDiagnostic,
    ArrayContainsToFrozenSetCodeFix,
    ArrayContainsToFrozenSetTests,
    DefaultVerifier>
{
}
