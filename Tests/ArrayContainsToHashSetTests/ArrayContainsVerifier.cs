using Collections.Analyzer.CodeFixes;
using Collections.Analyzer.Diagnostics.CI0008;
using Microsoft.CodeAnalysis.Testing;

namespace Tests.ArrayContainsToHashSetTests;

public class ArrayContainsVerifier : CodeFixVerifier<
    ArrayContainsToHashSetDiagnostic,
    ArrayContainsToHashSetCodeFix,
    ArrayContainsToHashSetTests,
    DefaultVerifier>
{
}