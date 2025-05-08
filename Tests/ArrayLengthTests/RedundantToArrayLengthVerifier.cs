using Collections.Analyzer.CodeFixes;
using Collections.Analyzer.Diagnostics.CI0005;
using Microsoft.CodeAnalysis.Testing;

namespace Tests.ArrayLengthTests;

internal sealed class RedundantToArrayLengthVerifier : CodeFixVerifier<ToArrayLengthDiagnostic,
    ReplaceWithCountCodeFix,
    RedundantToArrayLengthTests,
    DefaultVerifier>
{
}